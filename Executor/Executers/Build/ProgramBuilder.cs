﻿using Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Executor.Executers.Build
{
    abstract class ProgramBuilder
    {
        protected readonly ConcurrentQueue<Solution> solutionsQueue = new ConcurrentQueue<Solution>();
        private readonly Action<Guid, SolutionStatus> proccessSolution;
        private readonly Action<DirectoryInfo, Solution> finishBuildSolution;

        //protected abstract DirectoryInfo Build(Solution solution);
        protected abstract string ProgramFileName { get; }
        protected abstract string BuildFailedCondition { get; }
        protected abstract string GetBinariesDirectory(DirectoryInfo startDir);

        private Task buildingTask;
        private SemaphoreSlim buildingSemaphore;
        public ProgramBuilder(
            Action<Guid, SolutionStatus> proccessSolution,
            Action<DirectoryInfo, Solution> finishBuildSolution)
        {
            buildingSemaphore = new SemaphoreSlim(0, 1);
            buildingTask = Task.Run(BuildLoop);
            this.proccessSolution = proccessSolution;
            this.finishBuildSolution = finishBuildSolution;
        }
        public void Add(Solution solution)
        {
            if (solutionsQueue.Any(S => S.Id == solution.Id)) return;
            solutionsQueue.Enqueue(solution);
            try
            {
                buildingSemaphore.Release();
            }
            catch (SemaphoreFullException)
            {
            }
            catch { throw; }
        }
        private async Task BuildLoop()
        {
            while (true)
            {
                await buildingSemaphore.WaitAsync();
                while (solutionsQueue.TryDequeue(out var solution))
                {
                    solution.Status = SolutionStatus.InProcessing;
                    proccessSolution(solution.Id, SolutionStatus.InProcessing);
                    var result = Build(solution);
                    if (solution.Status == SolutionStatus.CompileError)
                    {
                        proccessSolution(solution.Id, SolutionStatus.CompileError);
                        continue;
                    }
                    finishBuildSolution(result, solution);
                }
            }
        }

        protected virtual DirectoryInfo Build(Solution solution)
        {
            var sourceDir = Directory.CreateDirectory(Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString()));
            Console.WriteLine($"new dir is {sourceDir.FullName}");
            File.WriteAllText(Path.Combine(sourceDir.FullName, ProgramFileName), solution.Raw, new UTF8Encoding(false));

            var proccess = new Process()
            {
                StartInfo = new ProcessStartInfo
                {
                    RedirectStandardError = true,
                    RedirectStandardOutput = true,
                    RedirectStandardInput = true,
                    FileName = "docker",
                    Arguments = $"run --rm -v {sourceDir.FullName}:/src/src builder:{Language}"
                },
            };

            SolutionStatus solStatus = SolutionStatus.InProcessing;
            proccess.OutputDataReceived += (D, E) => Proccess_OutputDataReceived(D, E, ref solStatus);
            proccess.ErrorDataReceived += (E, A) => Proccess_OutputDataReceived(E, A, ref solStatus);
            var success = proccess.Start();
            proccess.BeginErrorReadLine();
            proccess.BeginOutputReadLine();
            proccess.WaitForExit();
            if (solStatus == SolutionStatus.CompileError)
            {
                solution.Status = SolutionStatus.CompileError;
                return null;
            }
            var binPath = GetBinariesDirectory(sourceDir);
            var newBinPath = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.Move(binPath, newBinPath);
            return new DirectoryInfo(newBinPath);
        }
        private void Proccess_OutputDataReceived(object sender, DataReceivedEventArgs e, ref SolutionStatus status)
        {
            if (e.Data?.Contains(BuildFailedCondition) == true)
            {
                status = SolutionStatus.CompileError;
            }
        }
        private string lang;
        private string Language
        {
            get
            {
                return lang ??
                    (lang = GetType().GetCustomAttribute<LanguageAttribute>().Lang);
            }
        }
    }
}

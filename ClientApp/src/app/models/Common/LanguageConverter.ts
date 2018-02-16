export class LanguageConverter {
    private static readonly info: LanguageInfo[] =
    [
        {
            language: 'Java',
            fileExtension: '.java',
            webName: 'java'
        },
        {
            language: 'C#',
            fileExtension: '.cs',
            webName: 'csharp'
        },
        {
            language: 'Pascal ABC',
            fileExtension: '.pas',
            webName: 'pasabc'
        },
        {
            language: 'C',
            fileExtension: '.c',
            webName: 'c'
        },
        {
            language: 'C++',
            fileExtension: '.cpp',
            webName: 'cpp'
        },
        {
            language: 'Python 3.6.4',
            fileExtension: '.py',
            webName: 'python'
        }
    ];
    public static fileExtension(language: string) {
        return LanguageConverter.info.find(LI => LI.language === language).fileExtension;
    }

    public static languages(): string[] {
        return LanguageConverter.info.map(LI => LI.language);
    }

    public static webName(language: string) {
        return LanguageConverter.info.find(LI => LI.language === language).webName;
    }

    public static normalFromWeb(language: string) {
        return LanguageConverter.info.find(LI => LI.webName === language).language;
    }
}



class LanguageInfo {
    language: string;
    fileExtension: string;
    webName: string;
}

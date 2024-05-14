namespace VFM.Models.Help
{
    public class ErrorModel
    {
        public ErrorModel(string errorText)
        {
            ErrorText = errorText;
        }

        public string ErrorText { get; set; }

        public const string FileIsExist = "Ошибка: Файл существует";
        public const string DirectoryIsExist = "Ошибка: Директория уже существует";
        public const string LoginIsExist = "Ошибка: Аккаунт с таким логином уже существует";
        public const string AccountIsNotExist = "Ошибка: Такого аккаунта не существует";
        public const string AccountWithThisIDIsNotExist = "Ошибка: Аккаунта с таким ID не существует";
        public const string FilesOrDirectoriesIsNotExist = "Ошибка: Файл или папка не существует";

        public const string AllFieldsMostBeFields = "Ошибка: Все поля должны быть заполнены";

        public const string WrongLoginOrPassword = "Ошибка: Не верный логин или пароль";
        public const string WrongPath = "Ошибка: Не верный путь";

        public const string NotValidPassword = "Ошибка: Не верный формат пароля";
        public const string NotValidLogin = "Ошибка: Не верный формат логина";

        public const string CanNotUploadFiles = "Ошибка: Не удалось загрузить файлы";
        public const string CanNotDeleteFile = "Ошибка: Не удалось удалить файл";
        public const string CanNotDeleteDirectories = "Ошибка: Не удалось удалить папку";
        public const string CanNotChangeFileName = "Ошибка: Не удалось изменить название файла";
        public const string CanNotCreateFile = "Ошибка: Не удалось создать файл";
        public const string CanNotCreateDirectory = "Ошибка: Не удалось создать папку";
    }
}

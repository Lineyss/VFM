namespace VFM.Models
{
    public sealed class ErrorModel
    {
        public const string FileIsExist = "Ошибка: Файл существует";
        public const string DirectoryIsExist = "Ошибка: Директория уже существует";
        public const string FilesOrDirectoriesIsNotExist = "Ошибка: Файл или папка не существует";
        public const string AllFieldsMostBeFields = "Ошибка: Все поля должны быть заполнены";
        public const string WrongLoginAndPassword = "Ошибка: Не верный логин или пароль";
        public const string CanNotUploadFiles = "Ошибка: Не удалось загрузить файлы";
        public const string LoginIsExist = "Ошибка: Аккаунт с таким логином уже существует";
        public const string AccountIsNotExist = "Ошибка: Такого аккаунта не существует";
        public const string AccountWithThisIDIsNotExist = "Ошибка: Аккаунта с таким ID не существует";
    }
}

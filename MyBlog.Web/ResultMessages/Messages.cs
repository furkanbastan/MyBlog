using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace MyBlog.Web.ResultMessages;

public static class Messages
{
    public static string NotFound(string name)
    {
        return $"{name} bulunamadı...";
    }
    public static class Success
    {
        public static string Add(string name)
        {
            return $"{name} başarıyla eklenmiştir";
        }
        public static string Update(string name)
        {
            return $"{name} başarıyla güncellenmiştir.";
        }
        public static string Delete(string name)
        {
            return $"{name} başarıyla silinmiştir.";
        }
        public static string UndoDelete(string name)
        {
            return $"{name} başarıyla geri alınmıştır.";
        }
    }
    
    public static class Warning
    {
        public static string Add(string name)
        {
            return $"{name} eklenemedi.";
        }
        public static string Update(string name)
        {
            return $"{name} güncellenemedi.";
        }
        public static string Delete(string name)
        {
            return $"{name} silinemedi";
        }
        public static string UndoDelete(string name)
        {
            return $"{name} silinemedi";
        }
    }
}

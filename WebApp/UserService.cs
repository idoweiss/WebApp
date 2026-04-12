using System.Collections.Generic;
namespace WebApp
{

    public class UserService
    {
        // פעולה בונה סטאטית שתרוץ רק פעם אחת ויחידה במהלך הריצה של השרת המאתחלת את הטבלה והנתונים
        static UserService()
        {
            // יצירת טבלת משתמשים אם אינה קיימת
            DbHelper.RunSqlChange(@"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY,
                FullName TEXT NOT NULL,
                UserName TEXT UNIQUE NOT NULL,
                Password TEXT NOT NULL,
                Email TEXT UNIQUE NOT NULL
            );");


            // הכנסת נתוני דוגמה ראשוניים
            DbHelper.RunSqlChange(@"
            INSERT OR IGNORE INTO Users (FullName, UserName, Password, Email) VALUES
            ('Israel Israeli', 'israel1', 'p123', 'israel@example.com'),
            ('Noa Cohen', 'noac', 'p456', 'noa@example.com'),
            ('Yossi Levi', 'yossi_l', 'p789', 'yossi@example.com');");
        }


        // פעולה להחזרת כל המשתמשים מהמסד
        public List<User> GetAllUsers()
        {
            List<User> usersList = DbHelper.RunSelect<User>("SELECT * FROM Users");
            return usersList;
        }


        // מחיקת משתמש מהטבלה לפי מזהה
        public void DeleteUser(int id)
        {
            DbHelper.RunSqlChange($"DELETE FROM Users WHERE Id = {id}");
        }


        // עדכון כתובת אימייל עבור משתמש לפי מזהה
        public void UpdateEmailById(int id, string newEmail)
        {
            DbHelper.RunSqlChange($"UPDATE Users SET Email = '{newEmail}' WHERE Id = {id}");
        }


        // עדכון כתובת אימייל עבור משתמש לפי האימייל הישן שלו
        public void UpdateEmailByEmail(string oldEmail, string newEmail)
        {
            DbHelper.RunSqlChange($"UPDATE Users SET Email = '{newEmail}' WHERE Email = '{oldEmail}'");
        }
    }
}

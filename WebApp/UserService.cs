using System;
using System.Collections.Generic;

namespace WebApp
{
    public class UserService
    {
        // פעולה בונה סטאטית המאתחלת את הטבלה ומזינה נתוני דוגמה ראשוניים
        static UserService()
        {
            // יצירת טבלת המשתמשים במידה ואינה קיימת
            DbHelper.RunSqlChange(@"
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY,
                FullName TEXT NOT NULL,
                UserName TEXT UNIQUE NOT NULL,
                Password TEXT NOT NULL,
                Email TEXT UNIQUE NOT NULL
            );");

            // הזנת משתמש מנהל ונתוני דוגמה ראשוניים למערכת
            DbHelper.RunSqlChange(@"
            INSERT OR IGNORE INTO Users (FullName, UserName, Password, Email) VALUES
            ('מנהל מערכת', 'admin', 'admin123', 'admin@webapp.com'),
            ('Israel Israeli', 'israel1', 'p123', 'israel@example.com'),
            ('Noa Cohen', 'noac', 'p456', 'noa@example.com'),
            ('Yossi Levi', 'yossi_l', 'p789', 'yossi@example.com');");
        }

        // פעולה המחזירה רשימה של כל המשתמשים הקיימים במסד הנתונים
        public List<User> GetAllUsers()
        {
            List<User> usersList = DbHelper.RunSelect<User>("SELECT * FROM Users");
            return usersList;
        }

        // פעולה המוחקת משתמש ממסד הנתונים לפי המזהה שלו
        public void DeleteUser(int id)
        {
            DbHelper.RunSqlChange($"DELETE FROM Users WHERE Id = {id}");
        }

        // פעולה המעדכנת כתובת אימייל לפי מזהה משתמש
        public void UpdateEmailById(int id, string newEmail)
        {
            DbHelper.RunSqlChange($"UPDATE Users SET Email = '{newEmail}' WHERE Id = {id}");
        }

        // פעולה המוסיפה רשומה חדשה של משתמש למסד הנתונים
        public void AddNewUser(User newUser)
        {
            DbHelper.RunSqlChange($"INSERT INTO Users (FullName, UserName, Password, Email) VALUES ('{newUser.FullName}', '{newUser.UserName}', '{newUser.Password}', '{newUser.Email}')");
        }

        // פעולה המעדכנת את הפרטים של משתמש קיים במסד הנתונים
        public void UpdateUser(User user)
        {
            string sql = $"UPDATE Users SET FullName = '{user.FullName}', UserName = '{user.UserName}', Email = '{user.Email}' WHERE Id = {user.Id}";
            DbHelper.RunSqlChange(sql);
        }

        // פעולה הבודקת את פרטי ההתחברות ומחזירה את העצם של המשתמש אם נמצאה התאמה
        public User GetUserByLogin(string userName, string password)
        {
            string sql = $"SELECT * FROM Users WHERE UserName = '{userName}' AND Password = '{password}'";

            // השאילתה מחזירה תמיד רשימה של משתמשים, גם אם נמצאה רק התאמה אחת בבסיס הנתונים
            List<User> results = DbHelper.RunSelect<User>(sql);

            // בדיקה האם הרשימה שחזרה אינה ריקה, כלומר האם נמצא משתמש עם שם משתמש וסיסמה אלו
            if (results.Count > 0)
            {
                // כיוון שנמצאה התאמה, אנו ניגשים למקום הראשון ברשימה (אינדקס 0) ומחזירים את העצם שנמצא שם
                return results[0];
            }

            // אם הרשימה ריקה ולא נמצאה התאמה, הפעולה תחזיר ערך ריק המסמל שההתחברות נכשלה
            return null;
        }

        // פעולה השולפת מהמסד ומחזירה עצם של משתמש לפי המזהה הייחודי שלו
        public User GetUserById(int id)
        {
            string sql = $"SELECT * FROM Users WHERE Id = {id}";

            // שליפת הנתונים ושמירתם בתוך רשימה של עצמים מסוג משתמש
            List<User> results = DbHelper.RunSelect<User>(sql);

            // בדיקה האם נמצאה לפחות רשומה אחת בתוך הרשימה שתואמת למזהה ששלחנו
            if (results.Count > 0)
            {
                // גישה לאיבר הראשון ברשימה והחזרתו כתוצאה של הפעולה עבור המשתמש המבוקש
                return results[0];
            }

            // במקרה שלא נמצא משתמש עם המזהה המבוקש, נחזיר ערך ריק למניעת טעויות בהמשך הקוד
            return null;
        }
    }
}
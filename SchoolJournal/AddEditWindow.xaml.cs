using System.Windows;
using System.Data;
using System;
using MySql.Data.MySqlClient;

namespace SchoolJournal
{
    public partial class AddEditWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();
        private DataRowView row;

        public AddEditWindow()
        {
            InitializeComponent();
        }

        public AddEditWindow(DataRowView selectedRow) : this()
        {
            if (selectedRow == null) return;

            row = selectedRow;
            txtFirstName.Text = selectedRow["FirstName"].ToString();
            txtLastName.Text = selectedRow["LastName"].ToString();
            txtCourse.Text = selectedRow["CourseName"].ToString();
            txtGrade.Text = selectedRow["Grade"].ToString();

            if (DateTime.TryParse(selectedRow["Date"].ToString(), out DateTime date))
                dpDate.SelectedDate = date;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (!int.TryParse(txtGrade.Text, out int grade) || dpDate.SelectedDate == null)
                {
                    MessageBox.Show("Ошибка: Некорректная оценка или дата!");
                    return;
                }

                using (var conn = new MySqlConnection(db.connectionString))
                {
                    conn.Open();
                    using (var transaction = conn.BeginTransaction())
                    {
                        // 1. Обновление студента
                        string updateStudentQuery = @"
                    UPDATE Students 
                    SET FirstName = @FirstName, LastName = @LastName 
                    WHERE StudentID = @StudentID";
                        MySqlCommand studentCmd = new MySqlCommand(updateStudentQuery, conn, transaction);
                        studentCmd.Parameters.AddWithValue("@FirstName", txtFirstName.Text);
                        studentCmd.Parameters.AddWithValue("@LastName", txtLastName.Text);
                        studentCmd.Parameters.AddWithValue("@StudentID", row["StudentID"]);
                        studentCmd.ExecuteNonQuery();

                        // 2. Обновление курса
                        string updateCourseQuery = @"
                    UPDATE Courses 
                    SET CourseName = @CourseName 
                    WHERE CourseID = @CourseID";
                        MySqlCommand courseCmd = new MySqlCommand(updateCourseQuery, conn, transaction);
                        courseCmd.Parameters.AddWithValue("@CourseName", txtCourse.Text);
                        courseCmd.Parameters.AddWithValue("@CourseID", row["CourseID"]);
                        courseCmd.ExecuteNonQuery();

                        // 3. Обновление оценки
                        string updateGradeQuery = @"
                    UPDATE Grades 
                    SET Grade = @Grade, Date = @Date 
                    WHERE GradeID = @GradeID";
                        MySqlCommand gradeCmd = new MySqlCommand(updateGradeQuery, conn, transaction);
                        gradeCmd.Parameters.AddWithValue("@Grade", grade);
                        gradeCmd.Parameters.AddWithValue("@Date", dpDate.SelectedDate?.ToString("yyyy-MM-dd"));
                        gradeCmd.Parameters.AddWithValue("@GradeID", row["GradeID"]);
                        gradeCmd.ExecuteNonQuery();

                        transaction.Commit();
                    }
                }
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}");
            }
        }
    }
}
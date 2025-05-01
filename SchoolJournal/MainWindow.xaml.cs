using System.Windows;
using System.Data;

namespace SchoolJournal
{
    public partial class MainWindow : Window
    {
        private DatabaseHelper db = new DatabaseHelper();

        public MainWindow()
        {
            InitializeComponent();
            LoadData();
        }

        private void LoadData()
        {
            string query = @"
        SELECT 
            g.GradeID, 
            s.StudentID, 
            c.CourseID, 
            s.FirstName, 
            s.LastName, 
            c.CourseName, 
            g.Grade, 
            g.Date 
        FROM Grades g
        JOIN Students s ON g.StudentID = s.StudentID
        JOIN Courses c ON g.CourseID = c.CourseID";
            dgJournal.ItemsSource = db.GetData(query).DefaultView;
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            AddEditWindow window = new AddEditWindow();
            window.ShowDialog();
            LoadData();
        }

        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgJournal.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgJournal.SelectedItem;
                if (row["GradeID"] != null)
                {
                    AddEditWindow window = new AddEditWindow(row);
                    window.ShowDialog();
                    LoadData();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (dgJournal.SelectedItem != null)
            {
                DataRowView row = (DataRowView)dgJournal.SelectedItem;
                string query = $"DELETE FROM Grades WHERE GradeID = {row["GradeID"]}";
                db.ExecuteQuery(query);
                LoadData();
            }
        }
    }
}
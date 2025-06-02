using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace WindowsFormsApp1
{
    public partial class Form2 : Form
    {
        public int orderId;

        public Form2()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection(@"Data Source=ALEX\SQLEXPRESS; Initial Catalog=DOMOY; Integrated Security=true; TrustServerCertificate=true");

        private void Form2_Load(object sender, EventArgs e)
        {
            conn.Open();

            // Загружаем дисциплины, которые ведёт преподаватель
            string sql = @"
            SELECT d.Код_дисциплины, d.Название 
            FROM Нагрузка_преподавателя n
            JOIN Дисциплина d ON n.Код_дисциплины = d.Код_дисциплины
            JOIN Преподаватель p ON n.Номер_преподавателя = p.Номер_преподавателя
            WHERE p.Учетная_запись = @id";

            SqlDataAdapter da = new SqlDataAdapter(sql, conn);
            da.SelectCommand.Parameters.AddWithValue("@id", orderId);
            DataTable table = new DataTable();
            da.Fill(table);

            comboBox1.DataSource = table;
            comboBox1.DisplayMember = "Название";
            comboBox1.ValueMember = "Код_дисциплины";

            // Загружаем всех студентов
            SqlDataAdapter da2 = new SqlDataAdapter("SELECT Номер_студента, Фамилия + ' ' + Имя AS ФИО FROM Студент", conn);
            DataTable st = new DataTable();
            da2.Fill(st);
            comboBox2.DataSource = st;
            comboBox2.DisplayMember = "ФИО";
            comboBox2.ValueMember = "Номер_студента";

            // Загружаем группы
            SqlDataAdapter da3 = new SqlDataAdapter("SELECT Номер_группы, Название FROM Группа", conn);
            DataTable gr = new DataTable();
            da3.Fill(gr);
            comboBox3.DataSource = gr;
            comboBox3.DisplayMember = "Название";
            comboBox3.ValueMember = "Номер_группы";
        }

        // Просмотр оценок студента
        private void button1_Click(object sender, EventArgs e)
        {
            int studentId = (int)comboBox2.SelectedValue;

            SqlDataAdapter da = new SqlDataAdapter(@"
            SELECT d.Название AS Дисциплина, o.Оценка, o.Дата_оценки
            FROM Оценки o
            JOIN Дисциплина d ON o.Вид_дисциплины = d.Код_дисциплины
            WHERE o.Номер_студента = @id", conn);
            da.SelectCommand.Parameters.AddWithValue("@id", studentId);

            DataTable t = new DataTable();
            da.Fill(t);
            dataGridView1.DataSource = t;
        }

        // Просмотр оценок всей группы
        private void button2_Click(object sender, EventArgs e)
        {
            int groupId = (int)comboBox3.SelectedValue;

            SqlDataAdapter da = new SqlDataAdapter(@"
            SELECT s.Фамилия + ' ' + s.Имя AS Студент, d.Название AS Дисциплина, o.Оценка
            FROM Оценки o
            JOIN Студент s ON o.Номер_студента = s.Номер_студента
            JOIN Дисциплина d ON o.Вид_дисциплины = d.Код_дисциплины
            WHERE s.Номер_группы = @groupId", conn);
            da.SelectCommand.Parameters.AddWithValue("@groupId", groupId);

            DataTable t = new DataTable();
            da.Fill(t);
            dataGridView1.DataSource = t;
        }

        // Сохранение изменений
        private void button3_Click(object sender, EventArgs e)
        {
            int studentId = (int)comboBox2.SelectedValue;
            int disciplineId = (int)comboBox1.SelectedValue;

            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                if (row.IsNewRow) continue;

                object cell = row.Cells[1].Value; // Оценка
                int? grade = null;

                if (cell != null && int.TryParse(cell.ToString(), out int result))
                    grade = result;

                if (grade == null)
                {
                    SqlCommand del = new SqlCommand("DELETE FROM Оценки WHERE Номер_студента = @s AND Вид_дисциплины = @d", conn);
                    del.Parameters.AddWithValue("@s", studentId);
                    del.Parameters.AddWithValue("@d", disciplineId);
                    del.ExecuteNonQuery();
                }
                else
                {
                    SqlCommand up = new SqlCommand(@"
                    IF EXISTS (SELECT 1 FROM Оценки WHERE Номер_студента=@s AND Вид_дисциплины=@d)
                        UPDATE Оценки SET Оценка=@o WHERE Номер_студента=@s AND Вид_дисциплины=@d
                    ELSE
                        INSERT INTO Оценки (Номер_студента, Вид_дисциплины, Оценка) VALUES (@s, @d, @o)", conn);
                    up.Parameters.AddWithValue("@s", studentId);
                    up.Parameters.AddWithValue("@d", disciplineId);
                    up.Parameters.AddWithValue("@o", grade);
                    up.ExecuteNonQuery();
                }
            }
            MessageBox.Show("Оценки обновлены.");
        }

        private void Form2_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        // Показать студентов всех дисциплин, которые ведёт преподаватель
        private void button4_Click(object sender, EventArgs e)
        {
            SqlDataAdapter da = new SqlDataAdapter(@"
        SELECT DISTINCT s.Фамилия + ' ' + s.Имя AS Студент, g.Название AS Группа, d.Название AS Дисциплина
        FROM Нагрузка_преподавателя n
        JOIN Дисциплина d ON n.Код_дисциплины = d.Код_дисциплины
        JOIN Преподаватель p ON n.Номер_преподавателя = p.Номер_преподавателя
        JOIN Группа g ON n.Номер_группы = g.Номер_группы
        JOIN Студент s ON s.Номер_группы = g.Номер_группы
        WHERE p.Учетная_запись = @id", conn);

            da.SelectCommand.Parameters.AddWithValue("@id", orderId);
            DataTable t = new DataTable();
            da.Fill(t);
            dataGridView1.DataSource = t;
        }
    }
}


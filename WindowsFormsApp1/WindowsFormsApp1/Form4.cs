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
    public partial class Form4 : Form
    {
        public int orderId;
        public Form4()
        {
            InitializeComponent();
        }

        SqlConnection conn = new SqlConnection(@"Data Source=ALEX\SQLEXPRESS; Initial Catalog=DOMOY; Integrated Security=true; TrustServerCertificate=true");
        private void Form4_Load(object sender, EventArgs e)
        {
            conn.Open();

            string query = @"
            SELECT d.Код_дисциплины, d.Название 
            FROM Нагрузка_преподавателя n
            JOIN Дисциплина d ON n.Код_дисциплины = d.Код_дисциплины
            JOIN Преподаватель p ON n.Номер_преподавателя = p.Номер_преподавателя
            WHERE p.Учетная_запись = @id";

            SqlDataAdapter adapter = new SqlDataAdapter(query,conn);
            adapter.SelectCommand.Parameters.AddWithValue("@id", orderId);
            DataTable dt = new DataTable();
            adapter.Fill(dt);

            comboBox1.DataSource = dt;
            comboBox1.DisplayMember = "Название";
            comboBox1.ValueMember = "Код_дисциплины";

            SqlDataAdapter adapter1 = new SqlDataAdapter("SELECT Номер_группы, Название FROM Группа", conn);
            DataTable table1 = new DataTable();
            adapter1.Fill(table1);

            comboBox2.DataSource = table1;
            comboBox2.DisplayMember = "Название";
            comboBox2.ValueMember = "Номер_группы";


        }

        private void button1_Click(object sender, EventArgs e)
        {
            int groupId = (int)comboBox2.SelectedValue;

            SqlDataAdapter adapterGroup = new SqlDataAdapter(@"
            SELECT s.Фамилия + ' ' + s.Имя AS Студент, d.Название AS Дисциплина, o.Оценка
            FROM Оценки o
            JOIN Студент s ON o.Номер_студента = s.Номер_студента
            JOIN Дисциплина d ON o.Вид_дисциплины = d.Код_дисциплины
            WHERE s.Номер_группы = @groupId", conn);

            adapterGroup.SelectCommand.Parameters.AddWithValue("groupId", groupId);

            DataTable tableGroups = new DataTable();
            adapterGroup.Fill(tableGroups);

            dataGridView1.DataSource = tableGroups;
        }
    }
}

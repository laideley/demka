using System;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
namespace WindowsFormsApp1
{
    public partial class Form3 : Form
    {
        public int orderId;
        
        public Form3()
        {
            InitializeComponent();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(@"Data Source=ALEX\SQLEXPRESS; Initial Catalog=DOMOY; Integrated Security=true; TrustServerCertificate=true"))
            {
                conn.Open();

                string query = @"
                SELECT d.Название AS Дисциплина, o.Оценка, o.Дата_оценки
                FROM Оценки o
                JOIN Дисциплина d ON o.Вид_дисциплины = d.Код_дисциплины
                JOIN Студент s ON o.Номер_студента = s.Номер_студента
                WHERE s.Учетная_запись = @id";

                SqlDataAdapter adapter = new SqlDataAdapter(query, conn);
                adapter.SelectCommand.Parameters.AddWithValue("@id", orderId);

                DataTable table = new DataTable();
                adapter.Fill(table);
                dataGridView1.DataSource = table;
            }
        }
    }
}

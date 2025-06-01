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

namespace WindowsFormsApp1
{
    public partial class Form4 : Form
    {
        public Form4()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string login = textBox1.Text;
            string password = textBox2.Text;

            using(SqlConnection connection = new SqlConnection()) {
               
                connection.ConnectionString = @"Data Source=ALEX\SQLEXPRESS; Initial Catalog=DOMOY; Integrated Security=true; TrustServerCertificate=true";

                try
                {
                    connection.Open();

                    SqlCommand command = new SqlCommand("SELECT Роль, Код_пользователя FROM Пользователи WHERE Логин = @Login AND Пароль=@Password", connection);
                    command.Parameters.AddWithValue("@Login", login);
                    command.Parameters.AddWithValue("@Password", password);

                    SqlDataReader reader = command.ExecuteReader();

                    if(reader.HasRows)
                    {
                        while(reader.Read())
                        {
                            string role = reader["Роль"].ToString();
                            int userId = Convert.ToInt32(reader["Код_пользователя"]);

                            if(role == "Студент")
                            {
                                Form3 student = new Form3();
                                student.orderId = userId;
                                student.Show();
                            }

                            else if (role == "Преподаватель")
                            {
                                Form2 prepod = new Form2();
                                prepod.orderId = userId;
                                prepod.Show();
                            }
                        }
                        textBox1.Text = "";
                        textBox2.Text = "";
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("НЕПРАВИЛЬНЫЙ ЛОГИН ИЛИ ПАРОЛЬ");
                    }

                    reader.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }


            }
        }
    }
}

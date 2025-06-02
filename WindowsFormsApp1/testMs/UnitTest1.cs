using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using WindowsFormsApp1;
using System.Windows.Forms;


namespace testMs
{
    [TestClass]
    public class TestintLoginForm
    {
        [TestMethod]
        public void TestInvalidLogin()
        {
            var form = new Form1();
            form.textBox1.Text = "ooofg";
            form.textBox2.Text = "oooadds";
            form.button1_Click(null, null);

            Assert.IsFalse(form.LoginSuccessful);
        }

        [TestMethod]
        public void TestPositive()
        {
            var form = new Form1();
            form.textBox1.Text = "111";
            form.textBox2.Text = "111";
            form.button1_Click(null, null);
        }
    }
}

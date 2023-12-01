using Core.Models;
using Newtonsoft.Json;
using System.Text;


namespace ReserveFood_Client
{
    public partial class RegisterForm : Form
    {
        public RegisterForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(nameTxt.Text) || string.IsNullOrEmpty(personIdTxt.Text) || string.IsNullOrEmpty(familyTxt.Text))
                    MessageBox.Show("تمامی موارد را تکمیل کنید");

                else
                {
                    using (HttpClient client = new HttpClient())
                    {

                        var model = JsonConvert.SerializeObject(new { });
                        var data = new StringContent(model, Encoding.UTF8, "application/json");

                        var response = client.PostAsync(System.Configuration.ConfigurationSettings.AppSettings["URL"] + ":25565/api/GetOrder", data).Result;

                        if (response.IsSuccessStatusCode)
                        {
                            MessageBox.Show("با موفقیت ثبت شد");
                            System.Environment.Exit(1);
                        }
                        else
                        {
                            MessageBox.Show("با خطا مواجه شد");
                        }
                    }
                }
            }
            
          
            catch (Exception)
            {
                MessageBox.Show("خطای غیر منتظره");
            }
        }
        private void RegisterForm_Load(object sender, EventArgs e)
        {

        }

        private void backBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
        }
    }
}

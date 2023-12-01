using Core.Models;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace ReserveFood_Client
{
    public partial class Form1 : Form
    {
        List<Food> Foods = new List<Food>();
        public Form1()
        {
            InitializeComponent();
            Food1.Visible = false;
            Food2.Visible = false;
            Food3.Visible = false;
            Food4.Visible = false;
            Food5.Visible = false;
            Food6.Visible = false;
            Food7.Visible = false;
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = client.PostAsync(System.Configuration.ConfigurationSettings.AppSettings["URL"] + ":25566/api/Foods", null).Result;
                    response.EnsureSuccessStatusCode(); // Throw an exception if the response is not successful

                    Foods = response.Content.ReadFromJsonAsync<List<Food>>().Result;

                    if (Foods.Count == 0)
                    {
                        MessageBox.Show("لیست غذا امروز هنوز مشخص نشده است");
                        System.Environment.Exit(1);
                    }
                    else
                    {
                        if (Foods.Count > 0)
                        {
                            Food1.Visible = true;
                            Food1.Text = Foods[0].Name;
                            Food1.Checked = true;
                        }
                        if (Foods.Count > 1)
                        {
                            Food2.Visible = true;
                            Food2.Text = Foods[1].Name;
                        }
                        if (Foods.Count > 2)
                        {
                            Food3.Visible = true;
                            Food3.Text = Foods[2].Name;
                        }
                        if (Foods.Count > 3)
                        {
                            Food4.Visible = true;
                            Food4.Text = Foods[3].Name;
                        }
                        if (Foods.Count > 4)
                        {
                            Food5.Visible = true;
                            Food5.Text = Foods[4].Name;
                        }
                        if (Foods.Count > 5)
                        {
                            Food6.Visible = true;
                            Food6.Text = Foods[5].Name;
                        }
                        if (Foods.Count > 6)
                        {
                            Food7.Visible = true;
                            Food7.Text = Foods[6].Name;
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("سرور در دسترس نیست");
                    System.Environment.Exit(1);
                }
            }
        }

        private void BtnSend_Click(object sender, EventArgs e)
        {
            try
            {

                if (string.IsNullOrEmpty(PersonIdtxt.Text))
                    MessageBox.Show("کد پرسنلی خود را وارد کنید");

                else
                {
                    using (HttpClient client = new HttpClient())
                    {
                        var order = new Order() { FoodId = Food1.Checked ? 1 : Food2.Checked ? 2 : Food3.Checked ? 3 : Food4.Checked ? 4 : Food5.Checked ? 5 : Food6.Checked ? 6 : 7,PersonId=PersonIdtxt.Text };
                        var model = JsonConvert.SerializeObject(order);
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

        private void button1_Click(object sender, EventArgs e)
        {
         
                RegisterForm f = new RegisterForm(); // This is bad
                f.Show();
          
        }
    }
}
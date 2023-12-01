using System.Net.Sockets;
using System.Net;
using System.Windows.Forms;
using System.Reflection;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using Core.Models;

namespace ReserveFood_Server
{
    public partial class Form1 : Form
    {
        static string day = DateTime.Now.Year + "." + DateTime.Now.Year + "." + DateTime.Now.Year + ".";
        string OrderFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"Orders{day}.txt");
        string EmployeeFilePath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Employee.txt");
        Semaphore semaphore = new Semaphore(1, 1);
        int FoodId = 0;
        private List<Food> Foods = new List<Food>();
        public Form1()
        {
            //using (StreamWriter sw = File.AppendText(OrderFilePath))
            //{
            //}
            InitializeComponent();
            this.Food2.Visible = false;
            this.Food3.Visible = false;
            this.Food4.Visible = false;
            this.Food5.Visible = false;
            this.Food6.Visible = false;
            this.Food7.Visible = false;
            this.FoodName2.Visible = false;
            this.FoodName3.Visible = false;
            this.FoodName4.Visible = false;
            this.FoodName5.Visible = false;
            this.FoodName6.Visible = false;
            this.FoodName7.Visible = false;
            this.BtnAdd3.Visible = false;
            this.BtnAdd4.Visible = false;
            this.BtnAdd5.Visible = false;
            this.BtnAdd6.Visible = false;
            this.BtnAdd7.Visible = false;
            this.CountOrder1.Visible = false;
            this.CountOrder2.Visible = false;
            this.CountOrder3.Visible = false;
            this.CountOrder4.Visible = false;
            this.CountOrder5.Visible = false;
            this.CountOrder6.Visible = false;
            this.CountOrder7.Visible = false;
            BtnFood1.Visible = false;
            BtnFood2.Visible = false;
            BtnFood3.Visible = false;
            BtnFood4.Visible = false;
            BtnFood5.Visible = false;
            BtnFood6.Visible = false;
            BtnFood7.Visible = false;
            BtnFood0.Visible = false;
            this.dataGridView1.Visible = false;
            Task.Factory.StartNew(() => StartListeningForGetOrder());
            Task.Factory.StartNew(() => StartListeningForFoods());
            //StartListeningForGetOrder();
            //StartListeningForFoods();
        }

        private void BtnAdd2_Click(object sender, EventArgs e)
        {
            this.Food2.Visible = true;
            this.FoodName2.Visible = true;
            this.BtnAdd3.Visible = true;
        }

        private void BtnAdd3_Click(object sender, EventArgs e)
        {
            this.Food3.Visible = true;
            this.FoodName3.Visible = true;
            this.BtnAdd4.Visible = true;
        }

        private void BtnAdd4_Click(object sender, EventArgs e)
        {
            this.Food4.Visible = true;
            this.FoodName4.Visible = true;
            this.BtnAdd5.Visible = true;
        }

        private void BtnAdd5_Click(object sender, EventArgs e)
        {
            this.Food5.Visible = true;
            this.FoodName5.Visible = true;
            this.BtnAdd6.Visible = true;
        }

        private void BtnAdd6_Click(object sender, EventArgs e)
        {
            this.Food6.Visible = true;
            this.FoodName6.Visible = true;
            this.BtnAdd7.Visible = true;
        }

        private void BtnAdd7_Click(object sender, EventArgs e)
        {
            this.Food7.Visible = true;
            this.FoodName7.Visible = true;
        }

        private void SaveMenu_Click(object sender, EventArgs e)
        {
            Foods = new List<Food>();
            if (!string.IsNullOrEmpty(this.FoodName1.Text))
            {
                BtnFood1.Text = this.FoodName1.Text;
                BtnFood1.Visible = true;
                BtnFood0.Visible = true;
                Foods.Add(new Food() { Id = 1, Name = this.FoodName1.Text });
            }

            if (!string.IsNullOrEmpty(this.FoodName2.Text))
            {
                BtnFood2.Text = this.FoodName2.Text;
                BtnFood2.Visible = true;
                Foods.Add(new Food() { Id = 2, Name = this.FoodName2.Text });
            }

            if (!string.IsNullOrEmpty(this.FoodName3.Text))
            {
                BtnFood3.Text = this.FoodName3.Text;
                BtnFood3.Visible = true;
                Foods.Add(new Food() { Id = 3, Name = this.FoodName3.Text });
            }

            if (!string.IsNullOrEmpty(this.FoodName4.Text))
            {
                BtnFood4.Text = this.FoodName4.Text;
                BtnFood4.Visible = true;
                Foods.Add(new Food() { Id = 4, Name = this.FoodName4.Text });
            }

            if (!string.IsNullOrEmpty(this.FoodName5.Text))
            {
                BtnFood5.Text = this.FoodName5.Text;
                BtnFood5.Visible = true;
                Foods.Add(new Food() { Id = 5, Name = this.FoodName5.Text });
            }

            if (!string.IsNullOrEmpty(this.FoodName6.Text))
            {
                BtnFood6.Text = this.FoodName6.Text;
                BtnFood6.Visible = true;
                Foods.Add(new Food() { Id = 6, Name = this.FoodName6.Text });
            }

            if (!string.IsNullOrEmpty(this.FoodName7.Text))
            {
                BtnFood7.Text = this.FoodName7.Text;
                BtnFood7.Visible = true;
                Foods.Add(new Food() { Id = 7, Name = this.FoodName7.Text });
            }

            if (Foods.Count > 0)
            {
                dataGridView1.Visible = true;
                SetDataGrid(0);
            }
        }


        private async Task StartListeningForGetOrder()
        {
            var listener = new HttpListener();

            listener.Prefixes.Add(System.Configuration.ConfigurationSettings.AppSettings["URL"] + ":25565/");

            listener.Start();

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;
                semaphore.WaitOne();
                try
                {
                    if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/GetOrder")
                    {
                        Order model = new Order();
                        using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            model = JsonConvert.DeserializeObject<Order>(reader?.ReadToEnd());
                            if (model == null)
                            {
                                context.Response.StatusCode = 404;
                                context.Response.StatusDescription = "رکوئست خالی است.";
                                context.Response.Close();
                                semaphore.Release();
                                return;
                            }
                        }
                        var employee = new Employee();
                        using (FileStream fs = new FileStream(EmployeeFilePath, FileMode.OpenOrCreate))
                        {
                            using (StreamReader sw = new StreamReader(fs))
                            {

                                var file = await sw.ReadToEndAsync();
                                if (string.IsNullOrEmpty(file) || string.IsNullOrWhiteSpace(file))
                                {
                                    fs.Close();
                                    sw.Close();
                                    context.Response.StatusCode = 404;
                                    context.Response.StatusDescription = "شخص یافت نگردید.لطفا ثبت نام کنید";
                                    context.Response.Close();
                                    semaphore.Release();
                                    return;
                                }
                                var employees = JsonConvert.DeserializeObject<List<Employee>>(file);
                                employee = employees.FirstOrDefault(x => x.PersonId == model.PersonId);
                                if (employee == null)
                                {
                                    fs.Close();
                                    sw.Close();
                                    context.Response.StatusCode = 404;
                                    context.Response.StatusDescription = "شخص یافت نگردید.لطفا ثبت نام کنید";
                                    context.Response.Close();
                                    semaphore.Release();
                                    return;
                                }
                            }
                        }

                        using (StreamWriter sw = File.AppendText(OrderFilePath))
                        {
                            await sw.WriteLineAsync(JsonConvert.SerializeObject(model) + ",");
                        }
                        //string json = File.ReadAllText(OrderFilePath);

                        //List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(json);

                        //Order existOrder = null;
                        //if (orders != null && orders.Any())
                        //    orders.ForEach(x =>
                        //    {
                        //        if (x.Name == model.Name)
                        //            existOrder = x;
                        //    });

                        //if (existOrder != null)
                        //    existOrder.FoodId = model.FoodId;
                        //else
                        //{
                        //    if (orders == null)
                        //        orders = new List<Order>();

                        //    orders.Add(model);
                        //}

                        //string jsonWrite = JsonConvert.SerializeObject(orders);
                        //File.WriteAllText(OrderFilePath, jsonWrite);

                        context.Response.StatusCode = 200;
                        context.Response.Close();
                    }
                    if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/Register")
                    {
                        Employee model = new Employee();
                        using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                        {
                            model = JsonConvert.DeserializeObject<Employee>(reader?.ReadToEnd());
                            if (model == null)
                            {
                                context.Response.StatusCode = 404;
                                context.Response.StatusDescription = "رکوئست خالی است.";
                                context.Response.Close();
                                semaphore.Release();
                                return;
                            }
                        }
                        string json = File.ReadAllText(EmployeeFilePath);
                        var person = JsonConvert.DeserializeObject<List<Employee>>(json);

                        Employee existperson = null;
                        if (existperson != null && person.Any())
                            person.ForEach(x =>
                            {
                                if (x.PersonId == model.PersonId)
                                    existperson = x;
                            });

                        if (existperson != null)
                        {
                            person.Remove(existperson);
                        }
                        person.Add(model);

                        string jsonWrite = JsonConvert.SerializeObject(person);
                        File.WriteAllText(OrderFilePath, jsonWrite);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.Close();
                        semaphore.Release();
                    }
                }
                catch (Exception)
                {

                    throw;
                }
                finally
                {
                    semaphore.Release();
                }

            }
        }

        private void StartListeningForFoods()
        {
            var listener = new HttpListener();
            listener.Prefixes.Add(System.Configuration.ConfigurationSettings.AppSettings["URL"] + ":25566/");

            listener.Start();

            while (true)
            {
                var context = listener.GetContext();
                var request = context.Request;

                if (request.HttpMethod == "POST" && request.Url.AbsolutePath == "/api/Foods")
                {
                    using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                    {

                        string responseData = JsonConvert.SerializeObject(Foods);

                        byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseData);
                        context.Response.ContentLength64 = buffer.Length;
                        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
                    }

                    context.Response.StatusCode = 200;
                    context.Response.Close();
                }
                else
                {
                    context.Response.StatusCode = 404;
                    context.Response.Close();
                }
            }
        }

        private void BtnFood0_Click(object sender, EventArgs e)
        {
            SetDataGrid(0);
        }
        private void BtnFood1_Click(object sender, EventArgs e)
        {
            SetDataGrid(1);
        }

        private void BtnFood2_Click(object sender, EventArgs e)
        {
            SetDataGrid(2);
        }

        private void BtnFood3_Click(object sender, EventArgs e)
        {
            SetDataGrid(3);
        }

        private void BtnFood4_Click(object sender, EventArgs e)
        {
            SetDataGrid(4);
        }

        private void BtnFood5_Click(object sender, EventArgs e)
        {
            SetDataGrid(5);
        }

        private void BtnFood6_Click(object sender, EventArgs e)
        {
            SetDataGrid(6);
        }

        private void BtnFood7_Click(object sender, EventArgs e)
        {
            SetDataGrid(7);
        }

        private void RemoveOrders_Click(object sender, EventArgs e)
        {
            File.WriteAllText(OrderFilePath, null);
            SetDataGrid(0);
        }

        private void SetDataGrid(int? foodId)
        {
            if (foodId.HasValue)
                FoodId = foodId.Value;
            string json = File.ReadAllText(OrderFilePath);
            json = "[" + json + "]";
            List<Order> orders = JsonConvert.DeserializeObject<List<Order>>(json);
            if (foodId != 0)
                orders = orders?.Where(x => x.FoodId == FoodId)?.ToList();
            var group = orders.GroupBy(x => x.PersonId).Select(x => new { personId = x.Key, order = x.ToList() }).ToList();
            var repeatedOrder = new List<Order>();
            group.Where(x => x.order.Count > 1).ToList().ForEach(x =>
            {
                repeatedOrder.Add(x.order.OrderByDescending(x => x.OrderDate).FirstOrDefault());
                x.order.ForEach(y => orders.Remove(y));
            });
            orders.AddRange(repeatedOrder);
            var selected = orders?.Select(x => new OrderShow()
            {
                نام = x.Name,
                غذا = Foods?.FirstOrDefault(c => c.Id == x.FoodId)?.Name
            });
            dataGridView1.DataSource = selected?.ToList();
        }
    }
}
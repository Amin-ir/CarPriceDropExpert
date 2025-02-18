using oft_winform.DTO;
using oft_winform.Services;
using oft_winform.TemplateMethods;
using oft_winform.Utilities;
using System.Globalization;

namespace oft_winform
{
    public partial class Form1 : Form
    {
        PersianCalendar persianCalendar = new PersianCalendar();
        PDFGenerationTemplate pdfGenerationService => new TejaratPdfGenerationService();
        public Form1()
        {
            InitializeComponent();

            //carType_box.Text = "سواری";
            //chassisNumber_box.Text = "216A156GG";
            //engineNumber_box.Text = "158/13-AD2";
            //plate1_box.Value = 33;
            //plate2_combo.SelectedIndex = 5;
            //plate3_box.Value = 342;
            //plate4_box.Value = 16;
            //conventionalCarPrice_box.Value = 10_000_000_000;
            //manufactureYear_box.Value = 1400;
            //carPrice_box.Value = 8_000_000_000;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            date_lbl.Text = GetCurrentDateAsString();
        }

        private string GetCurrentDateAsString()
        {
            string day = persianCalendar.GetDayOfMonth(DateTime.Now).ToString("00");
            string month = persianCalendar.GetMonth(DateTime.Now).ToString("00");
            string year = persianCalendar.GetYear(DateTime.Now).ToString();

            return $"{year}/{month}/{day}";
        }

        private void printPdf_Click(object sender, EventArgs e)
        {
            try
            {
                ValidateTextFields();

                string outputPath = ".";
                Application.EnableVisualStyles();
                FolderBrowserDialog folderDialog = new FolderBrowserDialog
                {
                    Description = "محل ذخیره گزارش را انتخاب کنید",
                    ShowNewFolderButton = true // Allows creating a new folder
                };

                if (folderDialog.ShowDialog() == DialogResult.OK)
                {
                    string selectedPath = folderDialog.SelectedPath;
                    outputPath = selectedPath;
                }
                else
                    return;

                outputPath += $"/report-{DateTime.Now.ToString("yyyyMMdd")}-{DateTime.Now.ToString("HHmmss")}.pdf";
                
                string plate1 = plate1_box.Value.ToString().ReverseUsingArray();
                string plate2 = plate2_combo.SelectedItem.ToString();
                string plate3 = plate3_box.Value.ToString().ReverseUsingArray();
                string plate4 = plate4_box.Value.ToString().ReverseUsingArray() + " ایران";
                string plate = "\u202E" + string.Join('-', plate4, plate3, plate2, plate1);

                pdfGenerationService.GeneratePdf(new PdfGenerationRequest
                {
                    OutputPath = outputPath,
                    SelectedDamages = GetSelectedRadioInformation(),
                    CarPrice = (long)carPrice_box.Value,
                    ConventionalCarPrice = (long)conventionalCarPrice_box.Value,
                    CarType = carType_box.Text,
                    ChassisNumber = chassisNumber_box.Text,
                    EngineNumber = engineNumber_box.Text,
                    PlateNumber = plate, 
                    YearManufactured = (int)manufactureYear_box.Value,
                    Date = date_lbl.Text
                });

                MessageBox.Show($"گزارش در آدرس زیر ایجاد شد:\n{outputPath}");
            }
            catch (ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("خطای کنترل نشده رخ داد!");
            }
        }

        private Dictionary<string, DamageLevel> GetSelectedRadioInformation()
        {
            Dictionary<string, DamageLevel> result = new();

            Control containerControl = Controls.Find("containerPanel", true).First();

            foreach (var control in containerControl.Controls)
            {
                if (control is Panel panel)
                {
                    List<RadioButton> radios = GetChildControlsOfType<RadioButton>(panel);

                    RadioButton? selectedRadio = radios.FirstOrDefault(r => r.Checked);

                    DamageLevel selectedLevel = selectedRadio is null || room_change_4.Checked /*یعنی اتاق تعویض شده*/ ?
                        DamageLevel.فاقد_خسارت : Convert.ToInt32(selectedRadio.Name.Last() + "").ToEnum<DamageLevel>();

                    if (selectedRadio == room_change_4 && room_change_4.Checked) // دکمه تعویض اتاق در صورت انتخاب باید از قاعده بالا مستئنی باشد
                        selectedLevel = DamageLevel.خسارت_شدید;

                    result.Add(panel.Name.Split("Panel")[0], selectedLevel);
                }
            }
            return result;
        }

        private static List<T> GetChildControlsOfType<T>(Control c) where T : Control
        {
            var result = new List<T>();

            foreach (Control child in c.Controls)
            {
                if (child is T targetChild)
                    result.Add(targetChild);

                result.Union(GetChildControlsOfType<T>(child));
            }

            return result;
        }

        private void clean_Click(object sender, EventArgs e)
        {
            CleanRadios();
            CleanPlate();
            CleanEngineNumber();
            CleanChassisNumber();
            CleanCarType();
            CleanConventionalPrice();
            CleanManufactureYear();
            CleanPrice();
            CleanPriceDrop();
        }

        private void CleanPriceDrop()
        {
            priceDrop_lbl.Text = "";
        }

        private void CleanPrice()
        {
            carPrice_box.Value = 0;
        }

        private void CleanManufactureYear()
        {
            manufactureYear_box.Value = 1300;
        }

        private void CleanConventionalPrice()
        {
            conventionalCarPrice_box.Value = 0;
        }

        private void CleanCarType()
        {
            carType_box.Text = string.Empty;
        }

        private void CleanChassisNumber()
        {
            chassisNumber_box.Text = string.Empty;
        }

        private void CleanEngineNumber()
        {
            engineNumber_box.Text = string.Empty;
        }

        private void CleanPlate()
        {
            plate1_box.Value = 10;
            plate2_combo.SelectedIndex = 0;
            plate3_box.Value = 100;
            plate4_box.Value = 10;
        }

        private void CleanRadios()
        {
            GetRadios().ForEach(radio => radio.Checked = false);
        }

        private List<RadioButton> GetRadios()
        {
            List<RadioButton> result = new();

            Control containerControl = Controls.Find("containerPanel", true).First();

            foreach (var control in containerControl.Controls)
            {
                if (control is Panel panel)
                {
                    result = result.Union(GetChildControlsOfType<RadioButton>(panel))
                                   .ToList();
                }
            }

            return result;
        }

        private void room_change_2_CheckedChanged(object sender, EventArgs e)
        {
            List<RadioButton> radios = GetRadios();
            foreach (var radio in radios)
                if (radio != room_change_4 && radio != room_change_1)
                    radio.Enabled = room_change_1.Checked;
        }

        private void calculate_Click(object sender, EventArgs e)
        {
            try
            {
                Dictionary<string, DamageLevel> inputRadios = GetSelectedRadioInformation();
                long carPrice = Convert.ToInt64(carPrice_box.Value);
                long conventionalPrice = Convert.ToInt64(conventionalCarPrice_box.Value);
                int manufactureYear = Convert.ToInt32(manufactureYear_box.Value);

                float priceDrop = AppFormulas.GetPriceDrop(inputRadios, carPrice, manufactureYear, conventionalPrice);

                priceDrop_lbl.Text = $"{priceDrop:n0}";
            }catch(ArgumentException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch(ApplicationException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch(Exception ex)
            {
                MessageBox.Show("خطای کنترل نشده رخ داد.");
            }
        }

        private void manufactureYear_box_ValueChanged(object sender, EventArgs e)
        {
            if (persianCalendar.GetYear(DateTime.Now) - (int)manufactureYear_box.Value > 4)
            {
                room_change_1.Enabled = false;
                room_change_4.Enabled = false;
                room_change_1.Checked = false;
                room_change_4.Checked = false;
                GetRadios().ForEach(radio =>
                {
                    if (radio != room_change_1 && radio != room_change_4)
                        radio.Enabled = true;
                });
            }
            else
            {
                room_change_1.Enabled = true;
                room_change_4.Enabled = true;
            }
        }

        private void ValidateTextFields()
        {
            if (string.IsNullOrEmpty(carType_box.Text))
            {
                throw new ArgumentException("نوع خودرو وارد نشده است.");
            }
            
            if (string.IsNullOrEmpty(chassisNumber_box.Text))
            {
                throw new ArgumentException("شماره شاسی وارد نشده است.");
            }

            if (string.IsNullOrEmpty(engineNumber_box.Text))
            {
                throw new ArgumentException("شماره موتور وارد نشده است.");
            }

            if (plate2_combo.SelectedIndex == 0) 
            {
                throw new ArgumentException("پلاک وارد شده معتبر نمیباشد.");
            }
        }
    }
}

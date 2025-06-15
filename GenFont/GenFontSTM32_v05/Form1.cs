using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Windows.Forms;
//using System.Timers;

namespace EnhancedFontBitmapGenerator
{
    public partial class MainForm : Form
    {
        // Элементы управления формы
        private PictureBox pictureBox;
        private ListBox listHex;
        private ListBox listBinary;
        private TextBox txtSymbol;
        private ComboBox cmbFonts;
        private NumericUpDown nudSize;
        private Button btnGenerate;
        private Button btnExport;
        private Label lblHex;
        private Label lblBinary;
        private TabControl tabControl;

        //private Timer timer;
        //private Label lblNomSim;
        //private TextBox txtNomSim;
        private Label lblStartSim;
        private Label lblStopSim;
        private TextBox txtStartSim;
        private TextBox txtStopSim;
        private Button btnStartAvtoGenerate;

        public MainForm()
        {
            // Убрали вызов InitializeComponent()
            SetupControls(); // Новый метод инициализации
            LoadFonts();
            GenerateFontBitmap();
        }

        // Переименованный метод инициализации
        private void SetupControls()
        {
            // Основные настройки формы
            this.ClientSize = new Size(700, 650);
            this.Text = "Генератор шрифтов";
            this.Font = new Font("Segoe UI", 10);
            this.BackColor = SystemColors.Control;

            // Создаем элементы управления
            txtSymbol = new TextBox { Location = new Point(20, 20), Size = new Size(50, 25), Text = "A" };
            cmbFonts = new ComboBox { Location = new Point(80, 20), Size = new Size(150, 25), DropDownStyle = ComboBoxStyle.DropDownList };
            nudSize = new NumericUpDown { Location = new Point(240, 20), Size = new Size(60, 25), Minimum = 8, Maximum = 48, Value = 24 };
           
            btnGenerate = new Button { Location = new Point(310, 20), Size = new Size(90, 25), Text = "Ген символ" };
            btnStartAvtoGenerate = new Button { Location = new Point(410, 20), Size = new Size(90, 25), Text = "Ген всё" };
            btnExport = new Button { Location = new Point(510, 20), Size = new Size(90, 25), Text = "Экспорт" };
            pictureBox = new PictureBox { Location = new Point(20, 60), Size = new Size(200, 200), BorderStyle = BorderStyle.FixedSingle, SizeMode = PictureBoxSizeMode.Zoom };

            lblStartSim = new Label { Text = "Стартовый символ", Location = new Point(20, 280), AutoSize = true };
            txtStartSim = new TextBox { Location = new Point(20, 300), Size = new Size(50, 25), Text = "32" };

            lblStopSim = new Label { Text = "Стоповый символ", Location = new Point(20, 330), AutoSize = true };
            txtStopSim = new TextBox { Location = new Point(20, 350), Size = new Size(50, 25), Text = "126" };

            //lblNomSim = new Label { Text = "Номер символа", Location = new Point(20, 380), AutoSize = true };
            //txtNomSim = new TextBox { Location = new Point(20, 400), Size = new Size(50, 25), Text = "65" };

            //btnStartAvtoGenerate = new Button { Location = new Point(20, 450), Size = new Size(90, 25), Text = "Старт" };

            // Создаем вкладки для разных представлений
            tabControl = new TabControl { Location = new Point(240, 60), Size = new Size(420, 520) };

            // Вкладка с HEX-представлением
            TabPage tabHex = new TabPage("Hexadecimal");
            lblHex = new Label { Text = "24 lines × 3 bytes (HEX):", Dock = DockStyle.Top };
            listHex = new ListBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 10) };
            tabHex.Controls.Add(listHex);
            tabHex.Controls.Add(lblHex);

            // Вкладка с BINARY-представлением
            TabPage tabBinary = new TabPage("Binary");
            lblBinary = new Label { Text = "24 lines × 24 bits:", Dock = DockStyle.Top };
            listBinary = new ListBox { Dock = DockStyle.Fill, Font = new Font("Consolas", 10) };
            tabBinary.Controls.Add(listBinary);
            tabBinary.Controls.Add(lblBinary);

            tabControl.TabPages.Add(tabHex);
            tabControl.TabPages.Add(tabBinary);

            //Настройки таймера
            //timer = new Timer();
            //timer.Interval = 1000; // 1 секунда
            //timer.Tick += Timer_Tick;
            //timer.Stop();

            // Добавляем элементы на форму
            this.Controls.Add(txtSymbol);
            this.Controls.Add(cmbFonts);
            this.Controls.Add(nudSize);
            this.Controls.Add(btnGenerate);
            this.Controls.Add(btnExport);
            this.Controls.Add(pictureBox);
            this.Controls.Add(tabControl);

            //this.Controls.Add(lblNomSim);
            //this.Controls.Add(txtNomSim); //номер символа
            this.Controls.Add(lblStartSim);
            this.Controls.Add(txtStartSim); //
            this.Controls.Add(lblStopSim); //
            this.Controls.Add(txtStopSim); //
            this.Controls.Add(btnStartAvtoGenerate);

            // Обработчики событий
            btnGenerate.Click += (s, e) => ObrKnStartGeneratorSim();
            btnExport.Click += ExportData;
            txtSymbol.KeyPress += (s, e) => { if (e.KeyChar == '\r') GenerateFontBitmap(); };
            btnStartAvtoGenerate.Click += (s, e) => ObrKnStartAvtoGenerator();//генерашия шрифта

            
        }

        volatile int charCode; // Код символа 'A'
        int start_s;
        int stop_s;
        bool F_Start = false;

        private void ObrKnStartAvtoGenerator()
        { 

            if(!F_Start)
            {
                F_Start = true;
                start_s = Convert.ToInt32(txtStartSim.Text);
                stop_s = Convert.ToInt32(txtStopSim.Text);
                charCode = start_s;
                listHex.Items.Clear();
                listBinary.Items.Clear();
                Print_Sim_Displei();    
            }    
        }

        private void ObrKnStartGeneratorSim()
        {
            listHex.Items.Clear();
            listBinary.Items.Clear();
            GenerateFontBitmap();
        }

        //Обработка таймера
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Обновление UI (работает в основном потоке)
            //this.Text = $"Время: {DateTime.Now.ToString("HH:mm:ss")}";
            
            if(charCode >= start_s && charCode <stop_s+1)
            {
                //txtNomSim.Text = charCode.ToString();
                char c = (char)charCode;
                txtSymbol.Text = c.ToString();
                GenerateFontBitmap();
            }
           else
            {
                //timer.Stop();
                btnStartAvtoGenerate.Text = "Старт";
                F_Start = false;        
            }
            charCode++; // 
        }

        //Вывод символов на дисплей
        private void Print_Sim_Displei()
        {
            for (int i = start_s; i < stop_s + 1; i++)
            {
                char c = (char)i;
                txtSymbol.Text = c.ToString();
                //txtNomSim.Text = i.ToString();
                GenerateFontBitmap();
            }
            F_Start=false;
        }

        private void LoadFonts()
        {
            // Загрузка установленных шрифтов
            using (InstalledFontCollection fonts = new InstalledFontCollection())
            {
                foreach (FontFamily family in fonts.Families)
                {
                    if (family.IsStyleAvailable(FontStyle.Regular))
                    {
                        cmbFonts.Items.Add(family.Name);
                    }
                }
            }
            cmbFonts.SelectedItem = "Arial";
        }

        private void GenerateFontBitmap()
        {
            if (listHex == null || listBinary == null) return;

            
            listHex.Items.Add($" ");
            listHex.Items.Add($"// Символ {txtSymbol.Text}");
            //listHex.Items.Clear();
            //listBinary.Items.Clear();

            // Получаем параметры из UI
            string symbol = txtSymbol.Text.Length > 0 ? txtSymbol.Text[0].ToString() : "A";
            string fontName = cmbFonts.SelectedItem?.ToString() ?? "Arial";
            int fontSize = (int)nudSize.Value;

            try
            {
                // Создаем битмап 24x24
                using (Bitmap bitmap = new Bitmap(24, 24))
               
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;
                    graphics.Clear(Color.White);

                    // Создаем шрифт с выбранными параметрами
                    using (Font font = new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
                    {
                        StringFormat format = new StringFormat
                        {
                            Alignment = StringAlignment.Center,
                            //Alignment = StringAlignment.Center,
                            LineAlignment = StringAlignment.Center
                           //LineAlignment = StringAlignment.Near
                        };

                        graphics.DrawString(symbol, font, Brushes.Black,
                                            new Rectangle(0, 0, 24, 24),
                                            format);
                    }

                    // Отображаем изображение
                    if (pictureBox != null)
                        pictureBox.Image = (Image)bitmap.Clone();

                    // Формируем данные
                    for (int y = 0; y < 24; y++)
                    {
                        byte byte1 = 0;
                        byte byte2 = 0;
                        byte byte3 = 0;
                        StringBuilder binaryLine = new StringBuilder(24);

                        for (int x = 0; x < 24; x++)
                        {
                            Color pixel = bitmap.GetPixel(x, y);
                            bool isBlack = pixel.GetBrightness() < 0.5f;

                            if (isBlack)
                            {
                                int bitPosition = 23 - x;
                                if (bitPosition >= 16)
                                    byte1 |= (byte)(1 << (bitPosition - 16));
                                else if (bitPosition >= 8)
                                    byte2 |= (byte)(1 << (bitPosition - 8));
                                else
                                    byte3 |= (byte)(1 << bitPosition);

                                binaryLine.Append('1');
                            }
                            else
                            {
                                binaryLine.Append('0');
                            }
                        }

                        // Добавляем в списки
                        listHex.Items.Add($"0x{byte1:X2}, 0x{byte2:X2}, 0x{byte3:X2}, // " + binaryLine.ToString());
                        listBinary.Items.Add(binaryLine.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating bitmap: {ex.Message}", "Error",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


        private void ExportData(object sender, EventArgs e)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
                sfd.Title = "Export Bitmap Data";
                sfd.FileName = "Sim" + txtSymbol.Text+".txt";
                //sfd.FileName = "font_bitmap.txt";

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        //sb.AppendLine($"// Font: {cmbFonts.SelectedItem}, Size: {nudSize.Value}, Symbol: {txtSymbol.Text}");
                        //sb.AppendLine($"// Generated on {DateTime.Now}");
                        //sb.AppendLine();

                        //Новый код формирование заголовка
                        sb.AppendLine($"// Шрифт: {cmbFonts.SelectedItem}, Размер: {nudSize.Value}");
                        sb.AppendLine($"// вывод {DateTime.Now}");
                        sb.AppendLine($"// -------------------------------------------------------");
                        sb.AppendLine();
                        sb.AppendLine($"const uint8_t Font24_Table[] =");
                        sb.AppendLine($"{{");


                        // Добавляем HEX-представлени
                        //sb.AppendLine("HEX representation (24 lines × 3 bytes):");
                        foreach (var item in listHex.Items)
                        {
                            sb.AppendLine(item.ToString());
                        }

                        sb.AppendLine();
                        sb.AppendLine($"}}");
                        /*
                        // Добавляем BINARY-представление
                        sb.AppendLine("BINARY representation (24 lines × 24 bits):");
                        foreach (var item in listBinary.Items)
                        {
                            sb.AppendLine(item.ToString());
                        }
                        */

                        File.WriteAllText(sfd.FileName, sb.ToString());
                        MessageBox.Show("Data exported successfully!", "Export Complete",
                                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error exporting data: {ex.Message}", "Error",
                                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }

    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }
    }
}
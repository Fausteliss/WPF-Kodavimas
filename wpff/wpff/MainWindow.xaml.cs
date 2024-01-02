using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace wpff
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SelectFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedFileTextBox.Text = openFileDialog.FileName;
            }
        }

        private void EncryptButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = SelectedFileTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Pasirinkite failą ir įveskite slaptažodį.");
                return;
            }

            try
            {
                byte[] encryptedData = EncryptFile(filePath, password);
                string encryptedFilePath = filePath + ".užšifruotas";
                File.WriteAllBytes(encryptedFilePath, encryptedData);

                MessageBox.Show("Failas sėkmingai užšifruotas.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Šifruoti nepavyko: " + ex.Message);
            }
        }

        private void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string filePath = SelectedFileTextBox.Text;
            string password = PasswordTextBox.Password;

            if (string.IsNullOrWhiteSpace(filePath) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Pasirinkite failą ir įveskite slaptažodį.");
                return;
            }

            try
            {
                byte[] decryptedData = DecryptFile(filePath, password);
                string decryptedFilePath = Path.ChangeExtension(filePath, null);
                File.WriteAllBytes(decryptedFilePath, decryptedData);

                MessageBox.Show("Failas sėkmingai iššifruotas.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Iššifruoti nepavyko: " + ex.Message);
            }
        }

        private byte[] EncryptFile(string filePath, string password)
        {
            byte[] salt = Encoding.UTF8.GetBytes("SomeSaltValue");
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (AesManaged aes = new AesManaged())
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                {
                    fileStream.CopyTo(cryptoStream);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }

        private byte[] DecryptFile(string filePath, string password)
        {
            byte[] salt = Encoding.UTF8.GetBytes("SomeSaltValue");
            byte[] passwordBytes = Encoding.UTF8.GetBytes(password);

            using (AesManaged aes = new AesManaged())
            {
                Rfc2898DeriveBytes key = new Rfc2898DeriveBytes(passwordBytes, salt, 1000);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (FileStream fileStream = new FileStream(filePath, FileMode.Open))
                using (MemoryStream memoryStream = new MemoryStream())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                {
                    fileStream.CopyTo(cryptoStream);
                    cryptoStream.FlushFinalBlock();
                    return memoryStream.ToArray();
                }
            }
        }
    }
}
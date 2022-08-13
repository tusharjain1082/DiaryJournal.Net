﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiaryJournal.Net
{
    public static class userInterface
    {
        public static void GoFullscreen(Form form, bool fullscreen)
        {
            if (fullscreen)
            {
                form.FormBorderStyle = FormBorderStyle.None;
                form.WindowState = FormWindowState.Maximized;
            }
            else
            {
                form.WindowState = FormWindowState.Normal;
                form.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
            }
        }

        public static DialogResult ShowInputDialog(String title, ref string input)
        {
            System.Drawing.Size size = new System.Drawing.Size(800, 70);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = title;
            inputBox.StartPosition = FormStartPosition.CenterScreen;

            System.Windows.Forms.TextBox textBox = new TextBox();
            textBox.Size = new System.Drawing.Size(size.Width - 10, 23);
            textBox.Location = new System.Drawing.Point(5, 5);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(size.Width - 80 - 80, 39);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point(size.Width - 80, 39);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;
            return result;
        }

        public static DialogResult ShowDateTimeDialog(String title, ref DateTime dateTime, DateTime[]? boldedDates = null)
        {
            System.Drawing.Size size = new System.Drawing.Size(400, 270);
            Form inputBox = new Form();

            inputBox.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            inputBox.Text = title;
            inputBox.StartPosition = FormStartPosition.CenterScreen;

            System.Windows.Forms.MonthCalendar calendar = new MonthCalendar();
            calendar.Location = new System.Drawing.Point(5, 5);
            calendar.SelectionStart = dateTime;
            calendar.SelectionEnd = dateTime;
            calendar.BoldedDates = boldedDates;
            calendar.UpdateBoldedDates();
            inputBox.Controls.Add(calendar);

            System.Windows.Forms.DateTimePicker timepicker = new DateTimePicker();
            timepicker.Location = new System.Drawing.Point(5, 5 + calendar.Height + 10);
            timepicker.Value = dateTime;
            timepicker.Format = DateTimePickerFormat.Time;
            timepicker.ShowUpDown = true;
            inputBox.Controls.Add(timepicker);

            Button buttonNow = new Button();
            buttonNow.Name = "buttonNow";
            buttonNow.Size = new System.Drawing.Size(75, 23);
            buttonNow.Text = "&Now's time";
            buttonNow.Location = new System.Drawing.Point((5 + timepicker.Width + 5), timepicker.Top);
            buttonNow.Tag = timepicker;
            buttonNow.Click += ButtonNow_Click;
            inputBox.Controls.Add(buttonNow);

            Button okButton = new Button();
            okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new System.Drawing.Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new System.Drawing.Point(10, (inputBox.ClientSize.Height - okButton.Height) - 10);
            inputBox.Controls.Add(okButton);

            Button cancelButton = new Button();
            cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new System.Drawing.Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new System.Drawing.Point((okButton.Width + 10), (inputBox.ClientSize.Height - okButton.Height) - 10);
            inputBox.Controls.Add(cancelButton);

            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            DialogResult result = inputBox.ShowDialog();
            dateTime = new DateTime(calendar.SelectionStart.Year, calendar.SelectionStart.Month, 
                calendar.SelectionStart.Day, timepicker.Value.Hour, timepicker.Value.Minute, timepicker.Value.Second);

            return result;
        }

        private static void ButtonNow_Click(object? sender, EventArgs e)
        {
            Button? button = sender as Button;
            if (button == null)
                return;

            if (button.Tag == null)
                return;

            DateTimePicker timePicker = (DateTimePicker)button.Tag;
            if (timePicker == null)
                return;

            timePicker.Value = DateTime.Now;
        }
    }
}
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Desktoptale.Messages;
using Desktoptale.Messaging;

namespace Desktoptale
{
    public class DuplicateOptionsForm : Form
    {
        private static DuplicateOptionsForm instance;
        
        private IContainer components = null;
        private NumericUpDown NumInput;
        private Action<int> SuccessAction;
        private Action CancelAction;
        
        static DuplicateOptionsForm()
        {
            instance = new DuplicateOptionsForm();
            instance.StartPosition = FormStartPosition.CenterParent;
        }
        
        public static void Show(Action<int> successAction, Action cancelAction)
        {
            instance.SuccessAction = successAction;
            instance.CancelAction = cancelAction;
            
            try
            {
                MessageBus.Send(new GlobalPauseMessage() { Paused = true });
                instance.TopMost = true;
                instance.ShowDialog(FromHandle(WindowsUtils.MainWindowHwnd));
                instance.TopMost = false;
                MessageBus.Send(new GlobalPauseMessage() { Paused = false });
            }
            catch (InvalidOperationException e)
            {
                instance.Focus();
                return;
            }
            
            if (instance.DialogResult == DialogResult.OK)
            {
                instance.SuccessAction((int)instance.NumInput.Value);
            }
            else
            {
                instance.CancelAction();
            }
        }
        
        private DuplicateOptionsForm()
        {
            InitializeComponent();
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void OnOkClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }
        
        private void OnCancelClicked(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        private void InitializeComponent()
        {
            Panel PnlMain;
            Label LblMainText;
            Button BtnOk;
            Button BtnCancel;
            NumInput = new NumericUpDown();
            BtnOk = new Button();
            BtnCancel = new Button();
            PnlMain = new Panel();
            LblMainText = new Label();
            PnlMain.SuspendLayout();
            ((ISupportInitialize)(NumInput)).BeginInit();
            SuspendLayout();
            // 
            // PnlMain
            // 
            PnlMain.BackColor = SystemColors.Window;
            PnlMain.Controls.Add(NumInput);
            PnlMain.Controls.Add(LblMainText);
            PnlMain.Location = new Point(0, 0);
            PnlMain.Size = new Size(565, 115);
            PnlMain.TabIndex = 0;
            // 
            // NumInput
            // 
            NumInput.Location = new Point(429, 45);
            NumInput.Maximum = new decimal(new[] {
            10000,
            0,
            0,
            0});
            NumInput.Minimum = new decimal(new[] {
            1,
            0,
            0,
            0});
            NumInput.Name = "NumInput";
            NumInput.Size = new Size(110, 26);
            NumInput.TabIndex = 1;
            NumInput.Value = new decimal(new[] {
            1,
            0,
            0,
            0});
            // 
            // LblMainText
            // 
            LblMainText.AutoSize = true;
            LblMainText.Location = new Point(23, 45);
            LblMainText.Size = new Size(489, 40);
            LblMainText.TabIndex = 0;
            LblMainText.Text = "How many copies would you like to create?";
            LblMainText.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // BtnOk
            // 
            BtnOk.Location = new Point(298, 130);
            BtnOk.Name = "BtnOk";
            BtnOk.Size = new Size(113, 40);
            BtnOk.TabIndex = 1;
            BtnOk.Text = "OK";
            BtnOk.UseVisualStyleBackColor = true;
            BtnOk.Click += OnOkClicked;
            // 
            // BtnCancel
            // 
            BtnCancel.Location = new Point(426, 130);
            BtnCancel.Name = "BtnCancel";
            BtnCancel.Size = new Size(113, 40);
            BtnCancel.TabIndex = 2;
            BtnCancel.Text = "Cancel";
            BtnCancel.UseVisualStyleBackColor = true;
            BtnCancel.Click += OnCancelClicked;
            // 
            // FrmNumberSelect
            // 
            AutoScaleDimensions = new SizeF(9F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(565, 190);
            Controls.Add(BtnCancel);
            Controls.Add(BtnOk);
            Controls.Add(PnlMain);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MinimizeBox = false;
            MaximizeBox = false;
            Text = "Desktoptale";
            PnlMain.ResumeLayout(false);
            PnlMain.PerformLayout();
            ((ISupportInitialize)(NumInput)).EndInit();
            ResumeLayout(false);
            AcceptButton = BtnOk;
        }
    }
}
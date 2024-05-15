using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Desktoptale.Messages;
using Desktoptale.Messaging;

namespace Desktoptale
{
    public class CreatePartyForm : Form
    {
        private static CreatePartyForm instance;
        
        private IContainer components = null;
        private TextBox TextInput;
        private Action<string> SuccessAction;
        private Action CancelAction;
        
        static CreatePartyForm()
        {
            instance = new CreatePartyForm();
            instance.StartPosition = FormStartPosition.CenterParent;
        }
        
        public static void Show(Action<string> successAction, Action cancelAction)
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
                instance.SuccessAction(instance.TextInput.Text);
            }
            else
            {
                instance.CancelAction();
            }
        }
        
        private CreatePartyForm()
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
            TextInput = new TextBox();
            BtnOk = new Button();
            BtnCancel = new Button();
            PnlMain = new Panel();
            LblMainText = new Label();
            PnlMain.SuspendLayout();
            SuspendLayout();
            // 
            // PnlMain
            // 
            PnlMain.BackColor = SystemColors.Window;
            PnlMain.Controls.Add(TextInput);
            PnlMain.Controls.Add(LblMainText);
            PnlMain.Location = new Point(0, 0);
            PnlMain.Size = new Size(565, 115);
            PnlMain.TabIndex = 0;
            // 
            // NumInput
            // 
            TextInput.Location = new Point(329, 45);
            TextInput.Name = "TextInput";
            TextInput.Size = new Size(210, 26);
            TextInput.TabIndex = 1;
            TextInput.Text = "New Party";
            TextInput.Multiline = false;
            // 
            // LblMainText
            // 
            LblMainText.AutoSize = true;
            LblMainText.Location = new Point(23, 45);
            LblMainText.Size = new Size(489, 40);
            LblMainText.TabIndex = 0;
            LblMainText.Text = "What should the party be called?";
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
            ResumeLayout(false);
            AcceptButton = BtnOk;
        }
    }
}
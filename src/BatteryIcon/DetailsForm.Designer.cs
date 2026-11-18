namespace BatteryIcon
{
    partial class DetailsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rateLabel = new System.Windows.Forms.Label();
            this.rateValue = new System.Windows.Forms.Label();
            this.powerLineLabel = new System.Windows.Forms.Label();
            this.powerLineValue = new System.Windows.Forms.Label();
            this.designCapacityLabel = new System.Windows.Forms.Label();
            this.designCapacityValue = new System.Windows.Forms.Label();
            this.fullChargeCapacityLabel = new System.Windows.Forms.Label();
            this.fullChargeCapacityValue = new System.Windows.Forms.Label();
            this.healthLabel = new System.Windows.Forms.Label();
            this.healthValue = new System.Windows.Forms.Label();
            this.modelLabel = new System.Windows.Forms.Label();
            this.modelValue = new System.Windows.Forms.Label();
            this.buttonClose = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rateLabel
            // 
            this.rateLabel.AutoSize = true;
            this.rateLabel.Location = new System.Drawing.Point(12, 12);
            this.rateLabel.Name = "rateLabel";
            this.rateLabel.TabIndex = 0;
            this.rateLabel.Text = "Charge rate";
            // 
            // rateValue
            // 
            this.rateValue.AutoSize = true;
            this.rateValue.Location = new System.Drawing.Point(160, 12);
            this.rateValue.Name = "rateValue";
            this.rateValue.TabIndex = 1;
            this.rateValue.Text = "";
            // 
            // powerLineLabel
            // 
            this.powerLineLabel.AutoSize = true;
            this.powerLineLabel.Location = new System.Drawing.Point(12, 38);
            this.powerLineLabel.Name = "powerLineLabel";
            this.powerLineLabel.TabIndex = 2;
            this.powerLineLabel.Text = "Power line";
            // 
            // powerLineValue
            // 
            this.powerLineValue.AutoSize = true;
            this.powerLineValue.Location = new System.Drawing.Point(160, 38);
            this.powerLineValue.Name = "powerLineValue";
            this.powerLineValue.TabIndex = 3;
            this.powerLineValue.Text = "";
            // 
            // designCapacityLabel
            // 
            this.designCapacityLabel.AutoSize = true;
            this.designCapacityLabel.Location = new System.Drawing.Point(12, 64);
            this.designCapacityLabel.Name = "designCapacityLabel";
            this.designCapacityLabel.TabIndex = 4;
            this.designCapacityLabel.Text = "Design capacity";
            // 
            // designCapacityValue
            // 
            this.designCapacityValue.AutoSize = true;
            this.designCapacityValue.Location = new System.Drawing.Point(160, 64);
            this.designCapacityValue.Name = "designCapacityValue";
            this.designCapacityValue.TabIndex = 5;
            this.designCapacityValue.Text = "";
            // 
            // fullChargeCapacityLabel
            // 
            this.fullChargeCapacityLabel.AutoSize = true;
            this.fullChargeCapacityLabel.Location = new System.Drawing.Point(12, 90);
            this.fullChargeCapacityLabel.Name = "fullChargeCapacityLabel";
            this.fullChargeCapacityLabel.TabIndex = 6;
            this.fullChargeCapacityLabel.Text = "Full charge capacity";
            // 
            // fullChargeCapacityValue
            // 
            this.fullChargeCapacityValue.AutoSize = true;
            this.fullChargeCapacityValue.Location = new System.Drawing.Point(160, 90);
            this.fullChargeCapacityValue.Name = "fullChargeCapacityValue";
            this.fullChargeCapacityValue.TabIndex = 7;
            this.fullChargeCapacityValue.Text = "";
            // 
            // healthLabel
            // 
            this.healthLabel.AutoSize = true;
            this.healthLabel.Location = new System.Drawing.Point(12, 116);
            this.healthLabel.Name = "healthLabel";
            this.healthLabel.TabIndex = 8;
            this.healthLabel.Text = "Battery health";
            // 
            // healthValue
            // 
            this.healthValue.AutoSize = true;
            this.healthValue.Location = new System.Drawing.Point(160, 116);
            this.healthValue.Name = "healthValue";
            this.healthValue.TabIndex = 9;
            this.healthValue.Text = "";
            // 
            // modelLabel
            // 
            this.modelLabel.AutoSize = true;
            this.modelLabel.Location = new System.Drawing.Point(12, 142);
            this.modelLabel.Name = "modelLabel";
            this.modelLabel.TabIndex = 10;
            this.modelLabel.Text = "Model";
            // 
            // modelValue
            // 
            this.modelValue.AutoSize = true;
            this.modelValue.Location = new System.Drawing.Point(160, 142);
            this.modelValue.Name = "modelValue";
            this.modelValue.TabIndex = 11;
            this.modelValue.Text = "";
            // 
            // buttonClose
            // 
            this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonClose.Location = new System.Drawing.Point(160, 176);
            this.buttonClose.Name = "buttonClose";
            this.buttonClose.Size = new System.Drawing.Size(104, 29);
            this.buttonClose.TabIndex = 12;
            this.buttonClose.Text = "Close";
            this.buttonClose.UseVisualStyleBackColor = true;
            this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
            // 
            // DetailsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.buttonClose;
            this.ClientSize = new System.Drawing.Size(276, 217);
            this.Controls.Add(this.buttonClose);
            this.Controls.Add(this.rateLabel);
            this.Controls.Add(this.rateValue);
            this.Controls.Add(this.powerLineLabel);
            this.Controls.Add(this.powerLineValue);
            this.Controls.Add(this.designCapacityLabel);
            this.Controls.Add(this.designCapacityValue);
            this.Controls.Add(this.fullChargeCapacityLabel);
            this.Controls.Add(this.fullChargeCapacityValue);
            this.Controls.Add(this.healthLabel);
            this.Controls.Add(this.healthValue);
            this.Controls.Add(this.modelLabel);
            this.Controls.Add(this.modelValue);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DetailsForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Battery Details";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label rateLabel;
        private System.Windows.Forms.Label rateValue;
        private System.Windows.Forms.Label powerLineLabel;
        private System.Windows.Forms.Label powerLineValue;
        private System.Windows.Forms.Label designCapacityLabel;
        private System.Windows.Forms.Label designCapacityValue;
        private System.Windows.Forms.Label fullChargeCapacityLabel;
        private System.Windows.Forms.Label fullChargeCapacityValue;
        private System.Windows.Forms.Label healthLabel;
        private System.Windows.Forms.Label healthValue;
        private System.Windows.Forms.Label modelLabel;
        private System.Windows.Forms.Label modelValue;
        private System.Windows.Forms.Button buttonClose;
    }
}

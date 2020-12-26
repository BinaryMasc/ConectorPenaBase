namespace ConectorPenalisaFE
{
    partial class ConectorPanel
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
            this.button1 = new System.Windows.Forms.Button();
            this.button_Configurar = new System.Windows.Forms.Button();
            this.button_Visor = new System.Windows.Forms.Button();
            this.eventLog1 = new System.Diagnostics.EventLog();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(12, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(249, 28);
            this.button1.TabIndex = 0;
            this.button1.Text = "Iniciar";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button_Configurar
            // 
            this.button_Configurar.Location = new System.Drawing.Point(12, 46);
            this.button_Configurar.Name = "button_Configurar";
            this.button_Configurar.Size = new System.Drawing.Size(249, 27);
            this.button_Configurar.TabIndex = 1;
            this.button_Configurar.Text = "Configurar";
            this.button_Configurar.UseVisualStyleBackColor = true;
            this.button_Configurar.Click += new System.EventHandler(this.button_Configurar_Click);
            // 
            // button_Visor
            // 
            this.button_Visor.Location = new System.Drawing.Point(12, 79);
            this.button_Visor.Name = "button_Visor";
            this.button_Visor.Size = new System.Drawing.Size(249, 27);
            this.button_Visor.TabIndex = 2;
            this.button_Visor.Text = "Abrir Visor de Eventos";
            this.button_Visor.UseVisualStyleBackColor = true;
            this.button_Visor.Click += new System.EventHandler(this.button_Visor_Click);
            // 
            // eventLog1
            // 
            this.eventLog1.SynchronizingObject = this;
            // 
            // ConectorPanel
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 116);
            this.Controls.Add(this.button_Visor);
            this.Controls.Add(this.button_Configurar);
            this.Controls.Add(this.button1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "ConectorPanel";
            this.ShowIcon = false;
            this.Text = "ConectorPanel - ConectorFE";
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button_Configurar;
        private System.Windows.Forms.Button button_Visor;
        private System.Diagnostics.EventLog eventLog1;
        private System.Timers.Timer timer1;
    }
}
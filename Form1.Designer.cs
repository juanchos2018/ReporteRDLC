namespace TestRDL
{
    partial class Form1
    {
        /// <summary>
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.rdb_Grouped = new System.Windows.Forms.RadioButton();
            this.label1 = new System.Windows.Forms.Label();
            this.panelFilter = new System.Windows.Forms.Panel();
            this.rdb_NotGrouped = new System.Windows.Forms.RadioButton();
            this.cmbFilters = new System.Windows.Forms.ComboBox();
            this.panelFilter.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(46, 41);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(115, 40);
            this.button1.TabIndex = 0;
            this.button1.Text = "Generar RDL";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // rdb_Grouped
            // 
            this.rdb_Grouped.AutoSize = true;
            this.rdb_Grouped.Location = new System.Drawing.Point(205, 109);
            this.rdb_Grouped.Name = "rdb_Grouped";
            this.rdb_Grouped.Size = new System.Drawing.Size(104, 24);
            this.rdb_Grouped.TabIndex = 2;
            this.rdb_Grouped.TabStop = true;
            this.rdb_Grouped.Text = "Agrupado";
            this.rdb_Grouped.UseVisualStyleBackColor = true;
            this.rdb_Grouped.CheckedChanged += new System.EventHandler(this.rdb_Grouped_CheckedChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(115, 20);
            this.label1.TabIndex = 3;
            this.label1.Text = "Grupo Filtrado:";
            // 
            // panelFilter
            // 
            this.panelFilter.Controls.Add(this.cmbFilters);
            this.panelFilter.Controls.Add(this.label1);
            this.panelFilter.Location = new System.Drawing.Point(205, 41);
            this.panelFilter.Name = "panelFilter";
            this.panelFilter.Size = new System.Drawing.Size(326, 51);
            this.panelFilter.TabIndex = 4;
            // 
            // rdb_NotGrouped
            // 
            this.rdb_NotGrouped.AutoSize = true;
            this.rdb_NotGrouped.Location = new System.Drawing.Point(46, 109);
            this.rdb_NotGrouped.Name = "rdb_NotGrouped";
            this.rdb_NotGrouped.Size = new System.Drawing.Size(136, 24);
            this.rdb_NotGrouped.TabIndex = 5;
            this.rdb_NotGrouped.TabStop = true;
            this.rdb_NotGrouped.Text = "Mostrar Todos";
            this.rdb_NotGrouped.UseVisualStyleBackColor = true;
            // 
            // cmbFilters
            // 
            this.cmbFilters.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbFilters.FormattingEnabled = true;
            this.cmbFilters.Location = new System.Drawing.Point(135, 11);
            this.cmbFilters.Name = "cmbFilters";
            this.cmbFilters.Size = new System.Drawing.Size(176, 28);
            this.cmbFilters.TabIndex = 4;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(573, 158);
            this.Controls.Add(this.rdb_NotGrouped);
            this.Controls.Add(this.panelFilter);
            this.Controls.Add(this.rdb_Grouped);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.panelFilter.ResumeLayout(false);
            this.panelFilter.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.RadioButton rdb_Grouped;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panelFilter;
        private System.Windows.Forms.RadioButton rdb_NotGrouped;
        private System.Windows.Forms.ComboBox cmbFilters;
    }
}


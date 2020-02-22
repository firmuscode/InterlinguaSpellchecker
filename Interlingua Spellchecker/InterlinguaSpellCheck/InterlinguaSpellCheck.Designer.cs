﻿namespace InterlinguaSpellCheck
{
    partial class InterlinguaSpellCheck : Microsoft.Office.Tools.Ribbon.RibbonBase
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        public InterlinguaSpellCheck()
            : base(Globals.Factory.GetRibbonFactory())
        {
            InitializeComponent();
        }

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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tab1 = this.Factory.CreateRibbonTab();
            this.grpInterlinguaSpellCheck = this.Factory.CreateRibbonGroup();
            this.btnSpellCheck = this.Factory.CreateRibbonButton();
            this.tab1.SuspendLayout();
            this.grpInterlinguaSpellCheck.SuspendLayout();
            this.SuspendLayout();
            // 
            // tab1
            // 
            this.tab1.ControlId.ControlIdType = Microsoft.Office.Tools.Ribbon.RibbonControlIdType.Office;
            this.tab1.Groups.Add(this.grpInterlinguaSpellCheck);
            this.tab1.Label = "TabAddIns";
            this.tab1.Name = "tab1";
            // 
            // grpInterlinguaSpellCheck
            // 
            this.grpInterlinguaSpellCheck.Items.Add(this.btnSpellCheck);
            this.grpInterlinguaSpellCheck.Label = "Interlingua Proofing";
            this.grpInterlinguaSpellCheck.Name = "grpInterlinguaSpellCheck";
            // 
            // btnSpellCheck
            // 
            this.btnSpellCheck.ControlSize = Microsoft.Office.Core.RibbonControlSize.RibbonControlSizeLarge;
            this.btnSpellCheck.Image = global::InterlinguaSpellCheck.Properties.Resources.spellcheck;
            this.btnSpellCheck.Label = "Interlingua Spell Checker";
            this.btnSpellCheck.Name = "btnSpellCheck";
            this.btnSpellCheck.ScreenTip = "Interlingua Spell Checker";
            this.btnSpellCheck.ShowImage = true;
            this.btnSpellCheck.SuperTip = "Check the spelling of the Interlingua text in the document";
            this.btnSpellCheck.Click += new Microsoft.Office.Tools.Ribbon.RibbonControlEventHandler(this.btnSpellCheck_Click);
            // 
            // InterlinguaSpellCheck
            // 
            this.Name = "InterlinguaSpellCheck";
            this.RibbonType = "Microsoft.Word.Document";
            this.Tabs.Add(this.tab1);
            this.Load += new Microsoft.Office.Tools.Ribbon.RibbonUIEventHandler(this.InterlinguaSpellCheck_Load);
            this.tab1.ResumeLayout(false);
            this.tab1.PerformLayout();
            this.grpInterlinguaSpellCheck.ResumeLayout(false);
            this.grpInterlinguaSpellCheck.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        internal Microsoft.Office.Tools.Ribbon.RibbonTab tab1;
        internal Microsoft.Office.Tools.Ribbon.RibbonGroup grpInterlinguaSpellCheck;
        internal Microsoft.Office.Tools.Ribbon.RibbonButton btnSpellCheck;
    }

    partial class ThisRibbonCollection
    {
        internal InterlinguaSpellCheck InterlinguaSpellCheck
        {
            get { return this.GetRibbon<InterlinguaSpellCheck>(); }
        }
    }
}

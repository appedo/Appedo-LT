using AppedoLT.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Telerik.WinControls.UI;

namespace AppedoLT
{

    /// <summary>
    /// Used to give design UI.
    /// 
    /// Author: Rasith
    /// </summary>
    public partial class ucDesign : UserControl
    {
        private bool isDeleteHapped = false;
        private RepositoryXml _repositoryXml = RepositoryXml.GetInstance();
        private Common _common = Common.GetInstance();
        private Color _treeNodeDefaultColor;
        private static ucDesign _instance;

        /// <summary>
        /// To implement single instance class
        /// </summary>
        /// <returns></returns>
        public static ucDesign GetInstance()
        {
            if (_instance == null)
                _instance = new ucDesign();
            return _instance;
        }

        private ucDesign()
        {
            InitializeComponent();

            this.Dock = DockStyle.Fill;
            _treeNodeDefaultColor = tvRequest.BackColor;
            LoadTreeItem();

        }

        #region Design

        /// <summary>
        /// Load all script into tree view
        /// </summary>
        public void LoadTreeItem()
        {
            try
            {
                tvRequest.Nodes.Clear();
                
                //Read all script xml one by one
                foreach (string info in Directory.GetDirectories(".\\Scripts"))
                {
                    DirectoryInfo dicinfo = new DirectoryInfo(info);

                    if (File.Exists(info + "\\vuscript.xml"))
                    {
                        try
                        {
                            VuscriptXml vuscriptXml = new VuscriptXml(dicinfo.Name);
                            XmlNode vuscript = vuscriptXml.Doc.SelectSingleNode("//vuscript");
                            RadTreeNode vuScriptNode = new RadTreeNode();

                            vuScriptNode.Text = vuscript.Attributes["name"].Value;
                            vuScriptNode.Tag = vuscriptXml;
                            vuScriptNode.ImageKey = "scripts.gif";
                            foreach (XmlNode container in vuscript.ChildNodes)
                            {
                                RadTreeNode containerNode = new RadTreeNode();
                                containerNode.Text = container.Attributes["name"].Value;
                                containerNode.Tag = container;
                                
                                GetTreeNode(container, containerNode);
                                vuScriptNode.Nodes.Add(containerNode);
                            }
                            tvRequest.Nodes.Add(vuScriptNode);
                        }
                        catch (Exception ex)
                        {
                            ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        /// <summary>
        /// To create tree for each element in vuscript xml.
        /// </summary>
        /// <param name="ContainerNode"></param>
        /// <param name="parentNode"></param>
        public void GetTreeNode(XmlNode ContainerNode, RadTreeNode parentNode)
        {
            foreach (XmlNode action in ContainerNode.ChildNodes)
            {
                switch (action.Name)
                {
                    case "request":
                        RadTreeNode childContainerNode = new RadTreeNode();
                        childContainerNode.Text = action.Attributes["name"].Value;
                        childContainerNode.Tag = action;
                        if (action.Attributes["HasErrorResponse"] != null && Convert.ToBoolean(action.Attributes["HasErrorResponse"].Value) == true)
                        {
                            childContainerNode.BackColor = Color.Red;
                            parentNode.BackColor = Color.Red;
                            parentNode.ToolTipText = "Errors Found";
                        }
                        parentNode.Nodes.Add(childContainerNode);
                        break;
                    case "container":
                    case "if":
                    case "then":
                    case "else":
                    case "page":
                        RadTreeNode container = new RadTreeNode();
                        if (action.Name == "container" || action.Name == "page") container.Text = action.Attributes["name"].Value;
                        else if (action.Name == "if") container.Text = "If(Condition)";
                        else if (action.Name == "then") container.Text = "Then";
                        else if (action.Name == "else") container.Text = "else";
                        container.Tag = action;
                        GetTreeNode(action, container);
                        parentNode.Nodes.Add(container);
                        break;
                    case "loop":
                        RadTreeNode loop = new RadTreeNode();
                       // loop.Text = "Loop";
                        loop.Text = action.Attributes["name"].Value;
                        loop.Tag = action;
                        GetTreeNode(action, loop);
                        parentNode.Nodes.Add(loop);
                        break;
                    case "whileloop":
                        RadTreeNode whileloop = new RadTreeNode();
                        whileloop.Text = "WhileLoop";
                        whileloop.Tag = action;
                        GetTreeNode(action, whileloop);
                        parentNode.Nodes.Add(whileloop);
                        break;
                    case "delay":
                        RadTreeNode delayNode = new RadTreeNode();
                        delayNode.Text = "Delay";
                        delayNode.Tag = action;
                        parentNode.Nodes.Add(delayNode);
                        break;
                    case "starttransaction":
                        RadTreeNode transactionNode = new RadTreeNode();
                        transactionNode.Text = "StartTransaction";
                        transactionNode.Tag = action;
                        parentNode.Nodes.Add(transactionNode);
                        break;
                    case "endtransaction":
                        RadTreeNode endTransactionNode = new RadTreeNode();
                        endTransactionNode.Text = "EndTransaction";
                        endTransactionNode.Tag = action;
                        parentNode.Nodes.Add(endTransactionNode);
                        break;
                    case "javascript":
                        RadTreeNode javascriptNode = new RadTreeNode();
                        javascriptNode.Text = "JavaScript";
                        javascriptNode.Tag = action;
                        parentNode.Nodes.Add(javascriptNode);
                        break;
                    case "log":
                        RadTreeNode logNode = new RadTreeNode();
                        logNode.Text = "log";
                        logNode.Tag = action;
                        parentNode.Nodes.Add(logNode);
                        break;
                }
            }
        }

        /// <summary>
        /// To get available scripts
        /// </summary>
        /// <returns></returns>
        List<string> GetAvailableScript()
        {
            List<string> list = new List<string>();
            foreach (RadTreeNode node in tvRequest.Nodes)
            {
                list.Add(node.Text);
            }
            return list;
        }

        #region Flag Request

        private void searchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                XmlNode vuscript = ((VuscriptXml)tvRequest.SelectedNode.Tag).Doc.SelectSingleNode("//vuscript");
                string scriptId = vuscript.Attributes["id"].Value;
                FlagRequest fRequest;
                if (_repositoryXml.Doc.SelectSingleNode("//flag/flagrequest[@scriptid='" + scriptId + "']") == null)
                {
                    fRequest = new FlagRequest(null);
                    if (fRequest.ShowDialog() == DialogResult.OK)
                    {
                        fRequest.FlagRequestObj.Attributes.Append(_repositoryXml.GetAttribute("scriptid", scriptId));
                        _repositoryXml.Doc.SelectSingleNode("root/flag").AppendChild(fRequest.FlagRequestObj);
                    }
                }
                else
                {
                    fRequest = new FlagRequest(_repositoryXml.Doc.SelectSingleNode("//flag/flagrequest[@scriptid='" + scriptId + "']"));
                    if (fRequest.ShowDialog() == DialogResult.OK)
                    {
                        fRequest.FlagRequestObj.Attributes.Append(_repositoryXml.GetAttribute("scriptid", scriptId));
                    }
                }
                SetDefaultcolor(tvRequest.SelectedNode);
                if (vuscript.Attributes["type"].Value == "http")
                {
                    SetFlagRequestHttp(tvRequest.SelectedNode, fRequest.FlagRequestObj, scriptId);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void SetFlagRequestHttp(RadTreeNode request, XmlNode flagRequest, string scriptId)
        {
            if (request.Nodes.Count > 0)
            {
                foreach (RadTreeNode requestChild in request.Nodes)
                {
                    try
                    {
                        SetFlagRequestHttp(requestChild, flagRequest, scriptId);
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
                    }
                }
            }
            else
            {
                XmlNode requestNode = (XmlNode)request.Tag;
                if (requestNode.Name == "request")
                {
                    switch (flagRequest.Attributes["type"].Value)
                    {
                        case "requestheader":
                            {
                                StringBuilder headerText = new StringBuilder();

                                foreach (XmlNode header in requestNode.SelectSingleNode("headers").ChildNodes)
                                {
                                    headerText.AppendFormat("{0}: {1}\r\n", header.Attributes["name"].Value, header.Attributes["value"].Value);
                                }

                                if (flagRequest.Attributes["condition"].Value == "contain")
                                {
                                    if (headerText.ToString().Contains(flagRequest.Attributes["text"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                                else if (flagRequest.Attributes["condition"].Value == "notcontain")
                                {
                                    if (!headerText.ToString().Contains(flagRequest.Attributes["text"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                            }
                            break;

                        case "requestbody":
                            {
                                StringBuilder paramText = new StringBuilder();
                                if (requestNode.SelectSingleNode("params") != null)
                                {
                                    foreach (XmlNode param in requestNode.SelectSingleNode("./params").ChildNodes)
                                    {
                                        paramText.AppendFormat("{0}={1}&", param.Attributes["name"].Value, param.Attributes["value"].Value);
                                    }
                                }
                                if (requestNode.SelectSingleNode("querystringparams") != null)
                                {
                                    foreach (XmlNode param in requestNode.SelectSingleNode("querystringparams").ChildNodes)
                                    {
                                        paramText.AppendFormat("{0}={1}&", param.Attributes["name"].Value, param.Attributes["value"].Value);
                                    }
                                }

                                if (flagRequest.Attributes["condition"].Value == "contain")
                                {
                                    if (paramText.ToString().Contains(flagRequest.Attributes["text"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                                else if (flagRequest.Attributes["condition"].Value == "notcontain")
                                {
                                    if (!paramText.ToString().Contains(flagRequest.Attributes["text"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                            }
                            break;

                        case "responseheader":
                            {
                                StringBuilder headerText = new StringBuilder();
                                headerText.Append(requestNode.Attributes["ResponseHeader"].Value);

                                if (flagRequest.Attributes["condition"].Value == "contain")
                                {
                                    if (headerText.ToString().Contains(flagRequest.Attributes["text"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                                else if (flagRequest.Attributes["condition"].Value == "notcontain")
                                {
                                    if (!headerText.ToString().Contains(flagRequest.Attributes["text"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                            }
                            break;

                        case "responsebody":
                            {
                                StringBuilder respose = new StringBuilder();
                                respose.Append(Utility.GetFileContent(Constants.GetInstance().ExecutingAssemblyLocation + "\\Scripts\\" + scriptId + "\\" + requestNode.Attributes["resFilename"].Value));
                                if (flagRequest.Attributes["condition"].Value == "contain")
                                {
                                    if (respose.ToString().Contains(flagRequest.Attributes["text"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                                else if (flagRequest.Attributes["condition"].Value == "notcontain")
                                {
                                    if (!respose.ToString().Contains(flagRequest.Attributes["text"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                            }
                            break;

                        case "hasvariable":
                            {
                                StringBuilder paramText = new StringBuilder();
                                paramText.Append(requestNode.OuterXml);

                                if (requestNode.SelectSingleNode("querystringparams") != null)
                                {
                                    foreach (XmlNode param in requestNode.SelectSingleNode("querystringparams").ChildNodes)
                                    {
                                        paramText.AppendFormat("{0}={1}&", param.Attributes["name"].Value, param.Attributes["value"].Value);
                                    }
                                }

                                if (flagRequest.Attributes["condition"].Value == "contain")
                                {
                                    if (paramText.ToString().Contains(flagRequest.Attributes["hasvariablename"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                                else if (flagRequest.Attributes["condition"].Value == "notcontain")
                                {
                                    if (!paramText.ToString().Contains(flagRequest.Attributes["hasvariablename"].Value))
                                    {
                                        SetFlagRequstcolor(request);
                                    }
                                }
                            }
                            break;

                        case "haserror":
                            {
                                bool hasError = Convert.ToBoolean(requestNode.Attributes["HasErrorResponse"].Value);
                                if (hasError == true)
                                {
                                    SetFlagRequstcolor(request);
                                }
                            }
                            break;

                        case "disabled":
                            {
                                bool hasError = Convert.ToBoolean(requestNode.Attributes["IsEnable"].Value);
                                if (hasError == false)
                                {
                                    SetFlagRequstcolor(request);
                                }
                            }
                            break;

                        case "none":
                            {
                                //SetDefaultcolor(request);
                            }
                            break;

                    }
                }
            }

        }

        private void SetFlagRequstcolor(RadTreeNode node)
        {
            node.BackColor = Color.Aqua;
            if (node.Parent != null)
            {
                SetFlagRequstcolor(node.Parent);
            }
        }

        private void SetDefaultcolor(RadTreeNode node)
        {
            node.BackColor = _treeNodeDefaultColor;
            foreach (RadTreeNode child in node.Nodes)
            {
                SetDefaultcolor(child);
            }
        }

        #endregion

        #region MenuEvents

        private void tsContainer_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    string containerName = "Container";
                    XmlNode vuscipt = Constants.GetInstance().FindThirdRoot((XmlNode)tvRequest.SelectedNode.Tag);
                    if (vuscipt.SelectSingleNode(".//container[@name='" + containerName + "']") != null)
                    {
                        containerName = Constants.GetInstance().GetUniqueContainerName(containerName, vuscipt, 1);
                    }

                    XmlNode xmlNode = _common.CreateContainer(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument, containerName);
                    RadTreeNode treeNode = new RadTreeNode();
                    treeNode.Text = xmlNode.Attributes["name"].Value;
                    treeNode.Tag = xmlNode;

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, treeNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, treeNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(treeNode);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addDelayOnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    XmlNode xmlNode = _common.CreateDelay(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument);
                    RadTreeNode treeNode = new RadTreeNode();
                    treeNode.Text = "Delay";
                    treeNode.Tag = xmlNode;

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, treeNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, treeNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(treeNode);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addLoopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    string loopName = "Loop";
                    XmlNode vuscipt = Constants.GetInstance().FindThirdRoot((XmlNode)tvRequest.SelectedNode.Tag);
                    if (vuscipt.SelectSingleNode(".//loop[@name='" + loopName + "']") != null)
                    {
                        loopName = Constants.GetInstance().GetUniqueLoopName(loopName, vuscipt, 1);
                    }

                    XmlNode xmlNode = _common.CreateLoop(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument, loopName);
                    RadTreeNode treeNode = new RadTreeNode();
                    treeNode.Text = loopName;
                    treeNode.Tag = xmlNode;

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, treeNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, treeNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(treeNode);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addWhileLoopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    XmlNode xmlNode = _common.CreateWhileLoop(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument);
                    RadTreeNode treeNode = new RadTreeNode();
                    treeNode.Text = "WhileLoop";
                    treeNode.Tag = xmlNode;

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, treeNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, treeNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(treeNode);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addJavaScriptToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    XmlNode xmlNode = _common.CreateJavaScript(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument);
                    RadTreeNode treeNode = new RadTreeNode();
                    treeNode.Text = "JavaScript";
                    treeNode.Tag = xmlNode;

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, treeNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, treeNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(treeNode);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addIfThenElseMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    XmlNode xmlNode = _common.CreateIfThenElse(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument);

                    RadTreeNode IfContainerNode = new RadTreeNode();
                    IfContainerNode.Text = "If(Condition)";
                    IfContainerNode.Tag = xmlNode;

                    RadTreeNode ThenContainerNode = new RadTreeNode();
                    ThenContainerNode.Text = "Then";
                    ThenContainerNode.Tag = xmlNode.FirstChild;

                    RadTreeNode ElseContainerNode = new RadTreeNode();
                    ElseContainerNode.Text = "Else";
                    ElseContainerNode.Tag = xmlNode.LastChild;

                    IfContainerNode.Nodes.Add(ThenContainerNode);
                    IfContainerNode.Nodes.Add(ElseContainerNode);

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, IfContainerNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, IfContainerNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(IfContainerNode);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addStartTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    XmlNode xmlNode = _common.CreateStartTransaction(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument);
                    RadTreeNode treeNode = new RadTreeNode();
                    treeNode.Text = "StartTransaction";
                    treeNode.Tag = xmlNode;

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, treeNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, treeNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(treeNode);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void addEndTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    XmlNode xmlNode = _common.CreateEndTransaction(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument);
                    RadTreeNode treeNode = new RadTreeNode();
                    treeNode.Text = "EndTransaction";
                    treeNode.Tag = xmlNode;

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, treeNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, treeNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(treeNode);
                    }

                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        private void tsiLog_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNode != null)
                {
                    ToolStripMenuItem destination = (ToolStripMenuItem)sender;
                    XmlNode xmlNode = _common.CreateLog(((XmlNode)tvRequest.SelectedNode.Tag).OwnerDocument);
                    RadTreeNode treeNode = new RadTreeNode();
                    treeNode.Text = "Log";
                    treeNode.Tag = xmlNode;

                    if (destination.Name.EndsWith("After"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertAfter(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index + 1, treeNode);
                    }
                    else if (destination.Name.EndsWith("Before"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Parent.Tag).InsertBefore(xmlNode, (XmlNode)tvRequest.SelectedNode.Tag);
                        tvRequest.SelectedNode.Parent.Nodes.Insert(tvRequest.SelectedNode.Index, treeNode);
                    }
                    else if (destination.Name.EndsWith("Child"))
                    {
                        ((XmlNode)tvRequest.SelectedNode.Tag).AppendChild(xmlNode);
                        tvRequest.SelectedNode.Nodes.Add(treeNode);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        #endregion

        #region Events

        private void tvRequest_SelectedNodeChanged(object sender, RadTreeViewEventArgs e)
        {
            try
            {
                this.pnlMaster.Controls.Clear();
                if (tvRequest.SelectedNode != null)
                {
                    Color col = tvRequest.SelectedNode.BackColor;
                    tvRequest.SelectedNode.BackColor = col;
                    XmlNode node = tvRequest.SelectedNode.Level == 0 ? ((VuscriptXml)tvRequest.SelectedNode.Tag).Doc.SelectSingleNode("//vuscript") : (XmlNode)tvRequest.SelectedNode.Tag;
                    switch (node.Name)
                    {
                        case "request":
                            if (node.Attributes["Address"] != null)
                            {
                                #region HTTPRequest
                                this.pnlMaster.Controls.Add(UCHttpRequest.GetInstance().GetControl(
                                    node,
                                    tvRequest.SelectedNode));
                                #endregion
                            }
                            else
                            {
                                #region TCPRequest
                                this.pnlMaster.Controls.Add(UCTCPIPRequest.GetInstance().GetControl(node, tvRequest.SelectedNode));
                                #endregion
                            }
                            break;
                        case "vuscript":
                            this.pnlMaster.Controls.Add(ucScript.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "container":
                            this.pnlMaster.Controls.Add(ucContainer.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "page":
                            this.pnlMaster.Controls.Add(ucPage.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "delay":
                            this.pnlMaster.Controls.Add(ucDelay.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "javascript":
                            this.pnlMaster.Controls.Add(ucJavaScript.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "starttransaction":
                        case "endtransaction":
                            this.pnlMaster.Controls.Add(ucTransaction.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "loop":
                            this.pnlMaster.Controls.Add(ucLoop.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "whileloop":
                            this.pnlMaster.Controls.Add(ucWhileLoop.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "if":
                            this.pnlMaster.Controls.Add(ucIfThenElse.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                        case "log":
                            this.pnlMaster.Controls.Add(ucLog.GetInstance().GetControl(node, tvRequest.SelectedNode));
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private RadTreeNode GetRootParent(RadTreeNode node)
        {
            if (node.Parent == null) return node;
            else return GetRootParent(node.Parent);
        }
        private void tvRequest_DragEnding(object sender, RadTreeViewDragCancelEventArgs e)
        {
            try
            {
                XmlNode sorce = (XmlNode)e.Node.Tag;
                XmlNode des = (XmlNode)e.TargetNode.Tag;

                if (e.Node.Level < 2
                    || sorce.Name == "then"
                    || sorce.Name == "else"
                    || ((des.Name == "delay" || des.Name == "request" || des.Name == "if") && e.Direction == Telerik.WinControls.ArrowDirection.Right)
                    || ((des.Name == "then" || des.Name == "else" || e.TargetNode.Level < 2) && (e.Direction == Telerik.WinControls.ArrowDirection.Up || e.Direction == Telerik.WinControls.ArrowDirection.Down))
                    || (des.Name == "vuscript")
                    || (des.Name == "log")
                    || (des.ParentNode.Name == "page" && sorce.Name != "request")
                    || (des.Name == "page" && sorce.Name != "request" && e.Direction == Telerik.WinControls.ArrowDirection.Right)

                    )
                {
                    e.Cancel = true;
                    MessageBox.Show("Can't Move");
                }
                else
                {
                    if (e.Direction == Telerik.WinControls.ArrowDirection.Right)
                    {
                        des.AppendChild(sorce);

                    }
                    else if (e.Direction == Telerik.WinControls.ArrowDirection.Up)
                    {
                        des.ParentNode.InsertBefore(sorce, des);

                    }
                    else if (e.Direction == Telerik.WinControls.ArrowDirection.Down)
                    {
                        des.ParentNode.InsertAfter(sorce, des);

                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void btnDelete_Click(object sender, EventArgs e)
        {
            try
            {
                if (tvRequest.SelectedNodes.Count == 1)
                {
                    if (tvRequest.SelectedNode.Level == 0)
                    {
                        RadTreeNode parentNode = new RadTreeNode();
                        if (tvRequest.SelectedNode != null)
                        {

                            if (MessageBox.Show("Are you sure you want to delete selected vuscipt?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                DeleteScript((VuscriptXml)tvRequest.SelectedNode.Tag);
                                tvRequest.SelectedNode.Remove();
                                this.pnlMaster.Controls.Clear();
                            }
                        }
                    }
                    else
                    {
                        RadTreeNode parentNode = new RadTreeNode();
                        if (tvRequest.SelectedNode != null)
                        {
                            if (tvRequest.SelectedNode.Parent != null) parentNode = tvRequest.SelectedNode.Parent;
                            if (MessageBox.Show("Are you sure you want to delete selected " + ((XmlNode)tvRequest.SelectedNode.Tag).Name + "?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                            {
                                ((XmlNode)tvRequest.SelectedNode.Tag).ParentNode.RemoveChild((XmlNode)tvRequest.SelectedNode.Tag);
                                tvRequest.SelectedNode.Remove();
                                isDeleteHapped = true;
                                this.pnlMaster.Controls.Clear();
                            }
                        }
                    }
                }
                else if (tvRequest.SelectedNodes.Count > 1 && MessageBox.Show("Are you sure you want to delete selected items?", "Delete", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    for (; tvRequest.SelectedNodes.Count > 0; )
                    {

                        if (tvRequest.SelectedNode.Level == 0)
                        {
                            RadTreeNode parentNode = new RadTreeNode();
                            if (tvRequest.SelectedNode != null)
                            {

                                DeleteScript((VuscriptXml)tvRequest.SelectedNode.Tag);
                                tvRequest.SelectedNode.Remove();
                                this.pnlMaster.Controls.Clear();
                            }
                        }
                        else
                        {
                            RadTreeNode parentNode = new RadTreeNode();
                            if (tvRequest.SelectedNode != null)
                            {
                                if (tvRequest.SelectedNode.Parent != null) parentNode = tvRequest.SelectedNode.Parent;
                                ((XmlNode)tvRequest.SelectedNode.Tag).ParentNode.RemoveChild((XmlNode)tvRequest.SelectedNode.Tag);
                                tvRequest.SelectedNode.Remove();
                                isDeleteHapped = true;
                                this.pnlMaster.Controls.Clear();
                            }
                        }
                    }
                    this.pnlMaster.Controls.Clear();
                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void DeleteScript(VuscriptXml vuscriptXml)
        {
            string scriptid = vuscriptXml.Doc.SelectSingleNode("//vuscript").Attributes["id"].Value;
            if (Directory.Exists(".\\Scripts\\" + scriptid))
            {
                vuscriptXml = null;
                Directory.Delete(".\\Scripts\\" + scriptid, true);
            }
        }
        private void brnVariableManager_Click(object sender, EventArgs e)
        {
            frmVariableManager vm = new frmVariableManager();
            vm.ShowDialog();
        }
        private void btnRecord_Click_1(object sender, EventArgs e)
        {
            try
            {
                frmVUScriptNameHttp frm = new frmVUScriptNameHttp();
                frm.ShowDialog();
                if (frm.vuscriptXml != null)
                {
                    XmlNode vuscriptNode = frm.vuscriptXml.Doc.SelectSingleNode("//vuscript");
                    frmRecord rd = new frmRecord((Design)this.Parent.Parent.Parent, frm.name, vuscriptNode, frm.ddlParentContainer.SelectedIndex);
                    this.Parent.Parent.Parent.Visible = false;
                    rd.ShowDialog();
                    frm.vuscriptXml.Save();
                    RadTreeNode vuScriptNode = new RadTreeNode();
                    vuScriptNode.Text = vuscriptNode.Attributes["name"].Value;
                    vuScriptNode.Tag = frm.vuscriptXml;
                    vuScriptNode.ImageKey = "scripts.gif";
                    foreach (XmlNode container in vuscriptNode.ChildNodes)
                    {
                        RadTreeNode containerNode = new RadTreeNode();
                        containerNode.Text = container.Attributes["name"].Value;
                        containerNode.Tag = container;
                        GetTreeNode(container, containerNode);
                        vuScriptNode.Nodes.Add(containerNode);
                    }
                    tvRequest.Nodes.Add(vuScriptNode);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void btnTCPIPRecord_Click(object sender, EventArgs e)
        {
            try
            {
                frmVUScriptName frm = new frmVUScriptName("tcp");
                frm.ShowDialog();
                if (frm.vuscriptXml != null)
                {
                    XmlNode vuscriptNode = frm.vuscriptXml.Doc.SelectSingleNode("//vuscript");
                    frmTCPIPRecord frmTcpRecord = new frmTCPIPRecord((Design)this.Parent.Parent.Parent, vuscriptNode);
                    this.Parent.Parent.Parent.Visible = false;
                    frmTcpRecord.ShowDialog();

                    RadTreeNode vuScriptNode = new RadTreeNode();
                    vuScriptNode.Text = vuscriptNode.Attributes["name"].Value;
                    vuScriptNode.Tag = frm.vuscriptXml;
                    vuScriptNode.ImageKey = "scripts.gif";
                    foreach (XmlNode container in vuscriptNode.ChildNodes)
                    {
                        RadTreeNode containerNode = new RadTreeNode();
                        containerNode.Text = container.Attributes["name"].Value;
                        containerNode.Tag = container;
                        GetTreeNode(container, containerNode);
                        vuScriptNode.Nodes.Add(containerNode);
                    }
                    tvRequest.Nodes.Add(vuScriptNode);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            btnDelete_Click(null, null);
        }
        private void cntmUVScript_Opening(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
               findAndReplaceToolStripMenuItem.Visible= replaceServerToolStripMenuItem.Visible = searchToolStripMenuItem.Visible = insertAfterToolStripMenuItem.Visible = insertBeforeToolStripMenuItem.Visible = insertAsChildToolStripMenuItemChild.Visible = recordNewScripToolStripMenuItem.Visible = deleteToolStripMenuItem.Visible = false;

                if (tvRequest.SelectedNode.Level == 0)
                {
                    searchToolStripMenuItem.Visible = true;
                    replaceServerToolStripMenuItem.Visible = true;
                    findAndReplaceToolStripMenuItem.Visible = true;
                }
                if (tvRequest.SelectedNode != null)
                {
                    if (tvRequest.SelectedNode.Level == 0)
                    {
                        recordNewScripToolStripMenuItem.Visible = true;
                        deleteToolStripMenuItem.Visible = true;
                        return;
                    }
                    switch (((XmlNode)tvRequest.SelectedNode.Tag).Name)
                    {
                        case "vuscript":
                            recordNewScripToolStripMenuItem.Visible = true;
                            deleteToolStripMenuItem.Visible = true;
                            break;
                        case "container":
                        case "loop":
                        case "whileloop":
                            if (tvRequest.SelectedNode.Level == 1)
                            {
                                insertAsChildToolStripMenuItemChild.Visible = true;
                            }
                            else
                            {
                                insertAfterToolStripMenuItem.Visible = insertBeforeToolStripMenuItem.Visible = insertAsChildToolStripMenuItemChild.Visible = deleteToolStripMenuItem.Visible = true;
                            }
                            break;
                        case "request":
                            {
                                if (((XmlNode)tvRequest.SelectedNode.Tag).ParentNode.Name != "page") insertAfterToolStripMenuItem.Visible = insertBeforeToolStripMenuItem.Visible = true;
                                deleteToolStripMenuItem.Visible = true;

                            }
                            break;
                        case "page":
                        case "delay":
                        case "javascript":
                        case "log":
                        case "starttransaction":
                        case "endtransaction":
                        case "if":
                            {
                                if (((XmlNode)tvRequest.SelectedNode.Tag).ParentNode.Name != "page") insertAfterToolStripMenuItem.Visible = insertBeforeToolStripMenuItem.Visible = deleteToolStripMenuItem.Visible = true;

                            }
                            break;
                        case "then":
                        case "else":
                            {
                                insertAsChildToolStripMenuItemChild.Visible = true;
                            }
                            break;
                    }

                }

            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }
        private void tabsDesign_Resize(object sender, EventArgs e)
        {
            //tabsDesign.ItemsOffset = (tabsDesign.Width / 3) + 5;
        }
        private void mnuiVariableManager_Click(object sender, EventArgs e)
        {
            brnVariableManager_Click(null, null);
        }
        private void mnuiHttp_Click(object sender, EventArgs e)
        {
            btnRecord_Click_1(null, null);
        }
        private void mnuiTcpip_Click(object sender, EventArgs e)
        {
            btnTCPIPRecord_Click(null, null);
        }
        private void replaceServerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmReplaceServer frm = new frmReplaceServer((VuscriptXml)tvRequest.SelectedNode.Tag);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                bool Changed = false;
                foreach (ReplaceHost host in frm.HostList)
                {
                    XmlNode vuscrip = ((VuscriptXml)tvRequest.SelectedNode.Tag).Doc.SelectSingleNode("//vuscript");
                    if (vuscrip.Attributes["type"].Value == "http")
                    {
                        if (host.CurrentHost != host.NewHost || host.CurrentPort != host.NewPort || host.CurrentSchema != host.NewSchema)
                        {
                            if (Changed == false) Changed = true;

                            if (host.NewPort != null && host.NewPort.Length > 0 && host.NewPort != "")
                            {
                                vuscrip.InnerXml =
                                vuscrip.InnerXml.Replace(host.CurrentHost + ":" + host.CurrentPort, host.NewHost + ":" + host.NewPort)
                               .Replace(host.CurrentHost, host.NewHost)
                               .Replace("Port=\"" + host.CurrentPort + "\"", "Port=\"" + host.NewPort + "\"")
                               .Replace("Schema=\"" + host.CurrentSchema + "\"", "Schema=\"" + host.NewSchema + "\"");
                            }
                            else
                            {
                                vuscrip.InnerXml =
                                vuscrip.InnerXml.Replace(host.CurrentHost + ":" + host.CurrentPort, host.NewHost + host.NewPort)
                               .Replace(host.CurrentHost, host.NewHost)
                               .Replace("Port=\"" + host.CurrentPort + "\"", "Port=\"" + host.NewPort + "\"")
                               .Replace("Schema=\"" + host.CurrentSchema + "\"", "Schema=\"" + host.NewSchema + "\"");
                            }
                           
                        }
                    }
                    else if (vuscrip.Attributes["type"].Value == "tcp")
                    {
                        if (host.CurrentHost != host.NewHost || host.CurrentPort != host.NewPort)
                        {
                            if (Changed == false) Changed = true;
                            vuscrip.InnerXml =
                                 vuscrip.InnerXml.Replace(host.CurrentHost + ":" + host.CurrentPort, host.NewHost + ":" + host.NewPort)
                                .Replace(host.CurrentHost, host.NewHost)
                                .Replace("port=\"" + host.CurrentPort + "\"", "port=\"" + host.NewPort + "\"")
                                .Replace("serverip=\"" + host.CurrentPort + "\"", "serverip=\"" + host.NewPort + "\"");
                        }
                    }
                }
                if (Changed == true)
                {
                    ((VuscriptXml)tvRequest.SelectedNode.Tag).Save();
                    LoadTreeItem();
                }
            }
        }
        private void txtDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (Session.Login())
                {
                    TrasportData respose = null;
                    Dictionary<string, string> header = new Dictionary<string, string>();
                    string scripts = string.Empty;
                    header.Add("userid", Session.UserID);
                    Trasport server = new Trasport(Constants.GetInstance().UploadIPAddress, Constants.GetInstance().UploadPort);
                    server.Send(new TrasportData("AVAILABLESCRIPTS", string.Empty, header));
                    respose = server.Receive();
                    scripts = respose.Header["scripts"];
                    server.Close();
                    frmScriptNameList frm = new frmScriptNameList(GetAvailableScript(), scripts, this);
                    frm.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }
        private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvRequest.SelectedNode != null)
            {
                tvRequest.SelectedNode.ExpandAll();
            }
        }
        private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tvRequest.SelectedNode != null)
            {
                tvRequest.SelectedNode.CollapseAll();
            }
        }
        private void findAndReplaceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            frmFindAndReplace frm = new frmFindAndReplace(string.Empty, ((VuscriptXml)tvRequest.SelectedNode.Tag),this);
            frm.ShowDialog();
        }
        /// <summary>
        /// To update the container id after swiching one position to another position 
        /// </summary>
        /// <param name="scriptId"></param>
        public void updateContainerId(string scriptId)
        {
            XmlDocument document = null;
            XmlNodeList nodeList = null;
            XmlNode node = null;
            // Try and load xml data into an Xml document object and throw an
            // error message if this fails
            try
            {
                document = new XmlDocument();
           
                document.Load(@"./Scripts/"+scriptId+"/vuscript.xml");

                // Try and retrieve all book nodes
                nodeList = document.SelectNodes("/vuscript//container");
                List<int> listContainerId = new List<int>();
                foreach (XmlNode xNode in nodeList)
                {
                    listContainerId.Add(int.Parse(xNode.Attributes["id"].Value));
                }
                listContainerId.Sort();

                int i = 0;
                foreach (XmlNode xNode in nodeList)
                {
                    xNode.Attributes["id"].Value = listContainerId[i].ToString();
                    i++;
                }
                document.Save(@"./Scripts/" + scriptId + "/vuscript.xml");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);                
            }
        }
        /// <summary>
        /// This is to get all scriptid's from scripts folder
        /// </summary>
        public void getScriptIds()
        {
            try
            {               

                //Read all script xml one by one
                foreach (string info in Directory.GetDirectories(".\\Scripts"))
                {
                    DirectoryInfo dicinfo = new DirectoryInfo(info);

                    if (File.Exists(info + "\\vuscript.xml"))
                    {
                        updateContainerId(dicinfo.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.WritetoEventLog(ex.StackTrace + Environment.NewLine + ex.Message);
            }
        }

        public void btnScriptSave_Click(object sender, EventArgs e)
        {
            try
            {
                foreach (RadTreeNode script in tvRequest.Nodes)
                {
                    ((VuscriptXml)script.Tag).Save();                    
                }
                getScriptIds();
                if (sender != null) MessageBox.Show("Script is saved successfully");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        #endregion

        #endregion
       
    }
}
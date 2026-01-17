using BPRCPR.Resources.Common;
using BPRCPR.Resources.Operations;
using EmployeePortal.Utility;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace BPRCPR
{
    public partial class Ops_DOGH : System.Web.UI.Page
    {
        #region VARIABLES

        String _pageName = null;
        String _currMethodName = null;
        String _errorMessage = null;
        String _message = null;

        OracleConnection Oracon = new OracleConnection(ConfigurationManager.ConnectionStrings["OraDBConn_Replica_New"].ToString());
        SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["Connstr"].ToString());
        public OracleCommand _oracleCmd = null;

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                if (Session["UserID"] != null)
                {
                    if (!IsPostBack)
                    {

                    }
                }
                else
                {
                    Response.Redirect("Logout.aspx");
                }

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                _errorMessage = Common_Messages.ERR_MSG_FATAL_ISSUE;
                AuditLogger.RecordFatalLog(_pageName, _currMethodName, ex);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "popupScript", "swalalert('Error','" + _errorMessage + "','error');", true);

                return;
            }
        }
        public DataTable GetDOGHERPData(string APPLNO)
        {
            DataTable _dtResult = new DataTable();

            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                _oracleCmd = new OracleCommand("GET_DOGHDATA_BY_APPLNO", Oracon);
                _oracleCmd.CommandType = CommandType.StoredProcedure;
                _oracleCmd.Parameters.Add("@APPL_NO", OracleDbType.Varchar2, APPLNO, ParameterDirection.Input);
                _oracleCmd.Parameters.Add("DOGH_DATA", OracleDbType.RefCursor, ParameterDirection.Output);

                OracleDataAdapter _oracleDtAdptr = new OracleDataAdapter(_oracleCmd);

                _oracleDtAdptr.Fill(_dtResult);

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }

            return _dtResult;
        }
        public DataTable GetBorrowerCoNameAppl(string sAPPLNO)
        {
            DataTable dtBorrowerCoNameAppl = new DataTable();
            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                const string GET_BORROWER_COBORROWER_NAME = @"SELECT lms.CUSTOMER_NAME,lms.id,lms.CUSTOMER_NO,lms.CUSTOMER_TYPE,lms.FILENO,cor.CUSTOMER_NO AS COBORROWER_CUSTOMER_NO,
    fis.FULL_NAME AS COBORROWER_FULL_NAME
FROM lms_loanaccount_dtl lms
JOIN LMS_COBORROWERGUARANTOR_DTL cor 
    ON cor.LOANID = lms.id 
    AND cor.GUARANTOR_COAPPLICANT_TYPE_ID = 51002
JOIN Global_Customer_FIS fis 
    ON fis.CUSTOMER_INFO_FILE_NUMBER = cor.CUSTOMER_NO
WHERE lms.FILENO = :APPL_NO";
                OracleDataAdapter oracleAdapter = new OracleDataAdapter(GET_BORROWER_COBORROWER_NAME, Oracon);
                oracleAdapter.SelectCommand.Parameters.Add(new OracleParameter(":APPL_NO", sAPPLNO));
                oracleAdapter.SelectCommand.CommandTimeout = 0;
                oracleAdapter.Fill(dtBorrowerCoNameAppl);

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }
            return dtBorrowerCoNameAppl;
        }
        public DataTable GetAddressAppl(string sAPPLNO)
        {
            DataTable dtAddressAppl = new DataTable();
            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                const string GET_ADDRESS = @"SELECT COALESCE(""Address Line 1"", '') || ' ' || COALESCE(""Address Line 2"", '') AS ""Address"" FROM ""Insurance Nach Details"" a LEFT JOIN ""Customer"" b ON a.""Neo Customer ID"" = b.""Customer Number"" LEFT JOIN ""Address Details"" c ON c.""Customer Number"" = b.""Neo CIF ID"" AND c.""Address Type"" = 'ResidentialAddress' AND c.""Associated Loan Application ID"" = a.""APPLICATION_NUMBER"" WHERE UPPER(""Insured Party"") = 'YES' AND a.""APPLICATION_NUMBER"" = :APPL_NO";


                OracleDataAdapter oracleAdapter = new OracleDataAdapter(GET_ADDRESS, Oracon);
                oracleAdapter.SelectCommand.Parameters.Add(new OracleParameter(":APPL_NO", sAPPLNO));
                oracleAdapter.SelectCommand.CommandTimeout = 0;
                oracleAdapter.Fill(dtAddressAppl);

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }
            return dtAddressAppl;
        }
        public DataTable GetMaritalstatusAppl(string sCustomerNo)
        {
            DataTable dtMaritalstatusAppl = new DataTable();
            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                const string GET_BORROWER_COBORROWER_NAME = @"select MARITAL_STATUS_CODE from global_customer_fis WHERE CUSTOMER_INFO_FILE_NUMBER=:CUSTOMER_NO";
                OracleDataAdapter oracleAdapter = new OracleDataAdapter(GET_BORROWER_COBORROWER_NAME, Oracon);
                oracleAdapter.SelectCommand.Parameters.Add(new OracleParameter(":CUSTOMER_NO", sCustomerNo));
                oracleAdapter.SelectCommand.CommandTimeout = 0;
                oracleAdapter.Fill(dtMaritalstatusAppl);

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }
            return dtMaritalstatusAppl;
        }
        public DataTable GetERPData(string Query)
        {
            DataSet ds = new DataSet();

            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = Query;
                cmd.Connection = Oracon;
                OracleDataAdapter da = new OracleDataAdapter(cmd);
                cmd.CommandTimeout = 0;
                da.Fill(ds);

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }

            return ds.Tables[0];
        }
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                if (txtappNo.Text.Trim() != "")
                {
                    BindData(txtappNo.Text.Trim().ToUpper());
                }
                else
                {
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','Please Enter Loan Account Number.','top','center','animated zoomInRight','animated zoomInRight');", true);
                }

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }
        }

        public void BindData(string RefNo)
        {
            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                var Query1 = "select \"Application Number\" from \"Application\" where (upper(\"Current Status\") like '%DISBURSAL%' or upper(\"Application Status\") like '%DISBURSAL%')  and \"Application Number\"='" + RefNo.ToUpper().Trim() + "'";

                var check = GetERPData(Query1);
                if (RefNo.ToUpper().Contains("APP-") && (check == null || check.Rows.Count == 0))
                {
                    string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);
                    string query = "select name \"Application Number\" from " + Schema + ".application  where name='" + RefNo.ToUpper().Trim() + "' " +
                                   " and (status__c like '%Closed%' or status__c like '%Disbursal Completed%'  or status__c like '%Sanctioned%'  or status__c like '%Disbursal In-process%')";
                    Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                    builder.TrustServerCertificate = true;
                    builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                    builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                    builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);

                    builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                    builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                    builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]); ;
                    builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                    try
                    {
                        using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                        {
                            DataTable rsd = new DataTable();
                            Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                            da.Fill(rsd);
                            if (rsd != null && rsd.Rows.Count > 0)
                            {
                                check = rsd;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                if (check.Rows.Count == 0)
                {
                    MainDiv.Visible = false;

                    _errorMessage = Operations_Messages.ERR_MSG_APPL_NOT_FOUND;
                    AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);
                    return;
                }
                var dt = GetDOGHERPData(RefNo);
                DataTable dtBorrowerCoName = null;
                DataTable dtMaritalstatus = null;
                DataTable dtAddress = null;
                if (dt != null && dt.Rows.Count > 0)
                {
                    dtBorrowerCoName = GetBorrowerCoNameAppl(RefNo);
                    if(dtBorrowerCoName != null && dtBorrowerCoName.Rows.Count > 0)
                    {
                        string sCustomerNo = string.IsNullOrEmpty(dtBorrowerCoName.Rows[0]["CUSTOMER_NO"].ToString()) ? "" : dtBorrowerCoName.Rows[0]["CUSTOMER_NO"].ToString();
                        dtMaritalstatus = GetMaritalstatusAppl(sCustomerNo);
                    }
                    else
                    {
                        dtMaritalstatus = null;
                    }
                    dtAddress = GetAddressAppl(RefNo);

                }
                //Get BorrowerCoName From REDSHIFT
                if (RefNo.ToUpper().Contains("APP-") && (dt == null || dt.Rows.Count == 0))
                {
                    string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);
                    string query = "select a.name__c \"CUSTOMER_NAME\",ap.applicant_name__c \"COBORROWER_FULL_NAME\" from " + Schema + ".sf_loan_applicant ap left join " + Schema + ".application a on a.id = ap.application__c and ap.customer_type__c != 'Primary Applicant' where a.status__c in ('Disbursal Completed', 'Closed')and a.\"name\" = '" + RefNo.ToUpper().Trim() + "'";

                    Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                    builder.TrustServerCertificate = true;
                    builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                    builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                    builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);
                    builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                    builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                    //builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]);
                    builder.CommandTimeout = 0;
                    builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                    try
                    {
                        using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                        {
                            DataTable dtRSDBorrowerCo = new DataTable();
                            Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                            da.Fill(dtRSDBorrowerCo);
                            if (dtRSDBorrowerCo != null && dtRSDBorrowerCo.Rows.Count > 0)
                            {
                                dtBorrowerCoName = dtRSDBorrowerCo;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                        throw ex;
                    }
                }
                //Get Address From REDSHIFT
                if (RefNo.ToUpper().Contains("APP-") && (dt == null || dt.Rows.Count == 0))
                {
                    string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);
                    string query = "select SFLA.residence_address_line_1__c || ' ' || SFLA.residence_address_line_2__c  \"Address\" from " + Schema + ".application APP left join " + Schema + ".sf_loan_applicant SFLA on APP.insured_person__c = SFLA.id where APP.\"name\" = '" + RefNo.ToUpper().Trim() + "'";

                    Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                    builder.TrustServerCertificate = true;
                    builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                    builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                    builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);
                    builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                    builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                    //builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]);
                    builder.CommandTimeout = 0;
                    builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                    try
                    {
                        using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                        {
                            DataTable dtRSDAddressAppl = new DataTable();
                            Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                            da.Fill(dtRSDAddressAppl);
                            if (dtRSDAddressAppl != null && dtRSDAddressAppl.Rows.Count > 0)
                            {
                                dtAddress = dtRSDAddressAppl;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                        throw ex;
                    }
                }
                //Get Marital Status From REDSHIFT
                if (RefNo.ToUpper().Contains("APP-") && (dt == null || dt.Rows.Count == 0))
                {
                    string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);
                    string query = "select ap.marital_status__c \"MARITAL_STATUS_CODE\" from " + Schema + ".sf_loan_applicant ap left join " + Schema + ".application a on a.id = ap.application__c and ap.customer_type__c != 'Primary Applicant' where a.status__c in ('Disbursal Completed', 'Closed')and a.\"name\" = '" + RefNo.ToUpper().Trim() + "'";

                    Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                    builder.TrustServerCertificate = true;
                    builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                    builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                    builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);
                    builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                    builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                    //builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]);
                    builder.CommandTimeout = 0;
                    builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                    try
                    {
                        using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                        {
                            DataTable dtRSDMaritalstatus = new DataTable();
                            Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                            da.Fill(dtRSDMaritalstatus);
                            if (dtRSDMaritalstatus != null && dtRSDMaritalstatus.Rows.Count > 0)
                            {
                                dtMaritalstatus = dtRSDMaritalstatus;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                        throw ex;
                    }
                }

                if (RefNo.ToUpper().Contains("APP-") && (dt == null || dt.Rows.Count == 0))
                {
                    string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);
                    string query = " select APP.name \"Application Number\",APP.lms_response_reference__c \"Loan Account No\",                                                    " +
                                   "        SFLA.dob__c \"Date Of Birth\",G.gender__c \"Gender\",APP.nominee_party__c \"NP Name\",                                                " +
                                   "        APP.total_amount_recommended_pcac__c \"Sanction Loan Amount\", APP.name__c \"IP Name\",SFLA.\"name\" \"Neo Customer ID\",             " +
                                   "        APP.nominee_party_relationship_with_insured__c \"Nominee Relationship\",APP.branch_name__c \"Branch Name\",                           " +
                                   "        SFLA.lms_customer_info_file_number__c \"Neo CIF ID\",                                                                                 " +
                                   "        SFLA.residence_address_line_1__c ||' '|| SFLA.residence_address_line_2__c ||' '|| SFLA.residence_city__c ||' '|| SFLA.residence_state__c \"Address\" " +
                                   "        ,APP.branch_code__c \"Branch Code\"                                                                                                   " +
                                   "        ,fcc.Insurence_Code__c AS \"Ins_Provider\"                                                                                                   " +
                                   "        ,fcc.total_fee__c AS \"Premium_Amount\"                                                                                                   " +
                                   " from " + Schema + ".application APP                                                                                                          " +
                                   " left join " + Schema + ".sf_loan_applicant SFLA on APP.insured_person__c = SFLA.id                                                           " +
                                   " left join " + Schema + ".sf_account G on SFLA.customer_information__c = G.id                                                                 " +
                                   " left join " + Schema + ".sf_fee_creation fcc on APP.id = fcc.application__c and record_type_name__c = 'Insurance' and insurance_type__c = 'Disbursal Insurance'" +
                                   " where APP.\"name\" = '" + RefNo.ToUpper().Trim() + "'                                                                                        ";

                    Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                    builder.TrustServerCertificate = true;
                    builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                    builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                    builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);
                    builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                    builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                    builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]);
                    builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                    try
                    {
                        using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                        {
                            DataTable rsd = new DataTable();
                            Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                            da.Fill(rsd);
                            if (rsd != null && rsd.Rows.Count > 0)
                            {
                                dt = rsd;
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                        throw ex;
                    }
                }
                if (dt.Rows.Count > 0)
                {
                    MainDiv.Visible = true;

                    string ipState = dt.Rows[0]["Branch Code"].ToString();

                    if (RefNo.ToUpper().Contains("APP-"))
                    {
                        string HDFCIns = ConfigurationManager.AppSettings.AllKeys.Contains("DOGH_INS_HDFC")
                                        ? Convert.ToString(ConfigurationManager.AppSettings["DOGH_INS_HDFC"]) : string.Empty;

                        string KOTAKIns = ConfigurationManager.AppSettings.AllKeys.Contains("DOGH_INS_KOTAK") ? Convert.ToString(ConfigurationManager.AppSettings["DOGH_INS_KOTAK"]) : string.Empty;

                        string CREDITACCESS = ConfigurationManager.AppSettings.AllKeys.Contains("DOGH_INS_CREDITACCESS") ? Convert.ToString(ConfigurationManager.AppSettings["DOGH_INS_CREDITACCESS"]) : string.Empty;

                        string GODIGITIns = ConfigurationManager.AppSettings.AllKeys.Contains("DOGH_INS_GODIGIT") ? Convert.ToString(ConfigurationManager.AppSettings["DOGH_INS_GODIGIT"]) : string.Empty;

                        string MAXLIFEIns = ConfigurationManager.AppSettings.AllKeys.Contains("DOGH_INS_AXIS_MAXLIFE") ? Convert.ToString(ConfigurationManager.AppSettings["DOGH_INS_AXIS_MAXLIFE"]) : string.Empty;

                        if (string.IsNullOrEmpty(HDFCIns) || string.IsNullOrEmpty(KOTAKIns) || string.IsNullOrEmpty(CREDITACCESS) || string.IsNullOrEmpty(GODIGITIns)|| string.IsNullOrEmpty(MAXLIFEIns))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "popupScript", "swalalert('Info','Insurance provider not found in config','info');", true);
                            return;
                        }

                        string insProvider = dt.Rows[0]["Ins_Provider"].ToString();
                        if (string.IsNullOrEmpty(insProvider))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "popupScript", "swalalert('Info','Insurance provider not found for the provided Application Number','info');", true);
                            return;
                        }

                        if (insProvider.ToUpper().StartsWith(HDFCIns))
                        {
                            GenerateHDFCNewForm(dt, dtBorrowerCoName, dtMaritalstatus, dtAddress);
                        }
                        else if (insProvider.ToUpper().StartsWith(KOTAKIns))
                        {
                            GenerateKotakForm(dt);
                        }
                        else if (insProvider.ToUpper().StartsWith(CREDITACCESS))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "popupScript", "swal('Info', 'For \"credit access\" insurance, both filled and un-filled DOGH forms are not necessary.','info');", true);
                            return;
                        }
                        else if (insProvider.ToUpper().StartsWith(GODIGITIns))
                        {
                            GenerateGoDigitForm(dt);
                        }
                        else if (insProvider.ToUpper().StartsWith(MAXLIFEIns))
                        {
                            GenerateMaxLifeIns(dt, RefNo);
                        }
                        else
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "popupScript", "swalalert('Info','Invalid insurance provider','info');", true);
                            return;
                        }

                    }
                    if (RefNo.ToUpper().Contains("APPL"))
                    {
                        string allwdHDFCStates = ConfigurationManager.AppSettings.AllKeys.Contains("DOGH_CASAPPL_HDFC_STS") ? Convert.ToString(ConfigurationManager.AppSettings["DOGH_CASAPPL_HDFC_STS"]) : string.Empty;

                        if (string.IsNullOrEmpty(allwdHDFCStates))
                        {
                            ScriptManager.RegisterStartupScript(this, this.GetType(), "popupScript", "swalalert('Info','Allowed states are not found for HDFC Filled form in config','info');", true);
                            return;
                        }

                        List<string> allowedStates = new List<string>(allwdHDFCStates.Split(','));

                        if (!string.IsNullOrEmpty(ipState))
                        {
                            ipState = ipState.Substring(0, 2);
                        }
                        if (allowedStates.Contains(ipState))
                        {
                            GenerateHDFCNewForm(dt, dtBorrowerCoName, dtMaritalstatus, dtAddress);
                        }
                        else
                        {
                            GenerateKotakForm(dt);
                        }
                    }
                    else
                    {
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "popupScript", "swalalert('Info','Invalid Application Number','info');", true);
                        return;
                    }
                    

                    string LoanAccount = dt.Rows[0]["Loan Account No"].ToString();

                }
                else
                {
                    MainDiv.Visible = false;

                    _errorMessage = Operations_Messages.ERR_MSG_LOAN_NOT_FOUND;
                    AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);

                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);
                    return;
                }

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;  //ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','Error ! Please again try again.','top','center','animated zoomInRight','animated zoomInRight');", true);
            }
        }
        protected void GenerateKotakForm(DataTable dt)
        {
            try
            {
                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                string appNo = txtappNo.Text.Trim(); //dt.Rows[0]["Loan Account No"].ToString();
                                                     //string CIFID = dt.Rows[0]["Neo Customer ID"].ToString();
                string CIFID = dt.Rows[0]["Neo CIF ID"].ToString();

                if (Convert.ToString(dt.Rows[0]["IP Name"]) != "")
                {
                    string IPName = dt.Rows[0]["IP Name"].ToString();
                    if (Convert.ToString(dt.Rows[0]["NP Name"]) != "")
                    {
                        string NPName = dt.Rows[0]["NP Name"].ToString();
                        string BranchName = dt.Rows[0]["Branch Name"].ToString();

                        //DateTime dat = DateTime.ParseExact(dt.Rows[0]["Date Of Birth"].ToString(), "MM/dd/yyyy hh:mm:ss tt", CultureInfo.InvariantCulture);

                        //string s = dat.ToString("dd/M/yyyy", CultureInfo.InvariantCulture);

                        string DOB = string.Empty;
                        if (!string.IsNullOrEmpty(dt.Rows[0]["Date Of Birth"].ToString()))
                        {
                            DOB = Convert.ToDateTime(dt.Rows[0]["Date Of Birth"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        string Address = dt.Rows[0]["Address"].ToString();

                        string RelationShip = dt.Rows[0]["Nominee Relationship"].ToString();
                        string SanctionAmount = dt.Rows[0]["Sanction Loan Amount"].ToString();
                        string Gender = dt.Rows[0]["Gender"].ToString();


                        string path = Server.MapPath(@"~\Files\DOGH.jpg");
                        string Tick = Server.MapPath(@"~\Images\tick1.png");
                        //string unTick = Server.MapPath(@"Files\IndusINd\unTick.jpeg");

                        //int TotInstallAmt = Convert.ToInt32(InstallmentAmt) * 3;

                        Bitmap b = new Bitmap(path);
                        Graphics g = Graphics.FromImage(b);


                        // g.SmoothingMode = SmoothingMode.AntiAlias;


                        var date = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        System.Drawing.Font f = new System.Drawing.Font("Arial", 4, FontStyle.Regular);
                        System.Drawing.Font S_f = new System.Drawing.Font("Arial", 6, FontStyle.Bold);

                        //Sanction Amount
                        g.DrawString(SanctionAmount.ToString(), S_f, SystemBrushes.WindowText, new Point(330, 358));

                        // Loan Account No
                        // g.DrawString(LoanAccount, S_f, SystemBrushes.WindowText, new Point(180, 266));

                        // CIF ID
                        g.DrawString(CIFID, S_f, SystemBrushes.WindowText, new Point(1900, 560));

                        // IP Name
                        g.DrawString(IPName, S_f, SystemBrushes.WindowText, new Point(450, 615));

                        // DOB
                        g.DrawString(DOB, S_f, SystemBrushes.WindowText, new Point(2200, 615));

                        // Address
                        g.DrawString(Address, S_f, SystemBrushes.WindowText, new Point(360, 655));

                        // Gender
                        if (Gender == "Male")
                        {
                            g.DrawImage(System.Drawing.Image.FromFile(Tick), 2246, 663, 20, 20);
                        }
                        else
                        {
                            g.DrawImage(System.Drawing.Image.FromFile(Tick), 2340, 662, 20, 20);
                        }

                        // NP Name
                        g.DrawString(NPName, S_f, SystemBrushes.WindowText, new Point(340, 860));

                        // Relationship
                        g.DrawString(RelationShip, S_f, SystemBrushes.WindowText, new Point(600, 900));
                        string ROOT_PATH = ConfigurationManager.AppSettings["FILES_ROOT_PATH"];
                        string FILE_PATH = ConfigurationManager.AppSettings["DOGH_FOLDER"];
                        //FileInfo file = new FileInfo(Server.MapPath(@"~\Files\BOI\" + appNo + "_DOGH.jpeg"));
                        FileInfo file = new FileInfo(Server.MapPath(@"~\" + ROOT_PATH + @"\" + FILE_PATH + @"\" + appNo + "_DOGH.jpeg"));
                        if (file.Exists)//check file exsit or not  
                        {
                            file.Delete();
                        }

                        //b.RotateFlip(System.Drawing.RotateFlipType.Rotate90FlipNone);
                        //b.Save(Server.MapPath(@"Files\BOI\" + appNo + "_DOGH.jpeg"), ImageFormat.Jpeg);
                        b.Save(Server.MapPath(@"~\" + ROOT_PATH + @"\" + FILE_PATH + @"\" + appNo + "_DOGH.jpeg"), ImageFormat.Jpeg);

                        //Cimg.ImageUrl = "~/Files/BOI/" + appNo + "_DOGH.jpeg";
                        Cimg.ImageUrl = "~/" + ROOT_PATH + "/" + FILE_PATH + "/" + appNo + "_DOGH.jpeg";

                        //String.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapToBytes(b)));


                        // Response.Clear();
                        // Response.AddHeader("Content-Disposition", "attachment; filename=" + LoanAccount + "_DOGH.Jpeg ");
                        // Response.ContentType = "image/jpg";
                        //// zip.Save(Response.OutputStream);
                        // Response.TransmitFile(Server.MapPath(@"~\Files\BOI\"));
                        // Response.End();

                        Response.ContentType = "image/Jpeg";
                        Response.AppendHeader("Content-Disposition", "attachment; filename=" + appNo + "_DOGH.jpeg");
                        //Response.TransmitFile(Server.MapPath(@"~\Files\BOI\" + appNo + "_DOGH.jpeg"));
                        Response.TransmitFile(Server.MapPath(@"~\" + ROOT_PATH + @"\" + FILE_PATH + @"\" + appNo + "_DOGH.jpeg"));
                        //Response.End();
                        Response.Flush();
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        if (File.Exists(file.FullName))
                        {
                            file.Delete();
                        }
                    }
                    else
                    {
                        _errorMessage = Operations_Messages.ERR_MSG_NOMINEE_NOT_FOUND;
                        AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);

                        return;
                    }
                }
                else
                {
                    _errorMessage = Operations_Messages.ERR_MSG_INSUR_PER_NOT_FOUND;
                    AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);
                    return;
                }

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }
        }

        protected void GenerateHDFCForm(DataTable dt)
        {
            try
            {

                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                #region Box Fields points
                //Name
                int[] nameLeftPoints = { 200, 255, 310, 365, 420, 480, 535, 592, 645, 702, 760, 815, 870, 928, 985, 1035, 1095, 1148, 1205, 1265, 1318, 1375, 1432, 1488, 1545, 1602, 1658, 1712, 1770, 1822, 1880, 1935, 1990, 2047, 2100, 2160, 2217, 2272, 2330 };

                //Date of Birth
                int[] dobLeftPoints = { 310, 365, 420, 475, 533, 588, 645, 698 };

                //Address
                int[] addrRow1LeftPoints = { 476, 536, 586, 646, 701, 756, 816, 871, 926, 981, 1038, 1096, 1148, 1206, 1264, 1318, 1374, 1428, 1491, 1546, 1599, 1654, 1706, 1766, 1824, 1881, 1936, 1986, 2046, 2099, 2156, 2211, 2263, 2319 };

                int[] addrRow2LeftPoints = { 200, 255, 312, 368, 425, 480, 540, 590, 650, 705, 760, 820, 875, 930, 985, 1042, 1100, 1152, 1210, 1268, 1322, 1378, 1432, 1495, 1550, 1603, 1658, 1710, 1770, 1828, 1885, 1940, 1990, 2050, 2103, 2160, 2215, 2267, 2323 };

                //IP Mobile number
                int[] ipMobileLeftPoints = { 1295, 1350, 1410, 1463, 1518, 1577, 1630, 1688, 1745, 1800 };

                //IP State
                int[] ipStateLeftPoints = { 980, 1040, 1095, 1150, 1210, 1265, 1323, 1377, 1432, 1484, 1545, 1597, 1655, 1715, 1768, 1825 };

                //IP PAN
                int[] ipPANLeftPoints = { 1590, 1644, 1700, 1757, 1812, 1870, 1927, 1983, 2033, 2093 };

                //IP Pincode
                int[] ipPincodeLeftPoints = { 2042, 2102, 2160, 2215, 2270, 2326 };

                #endregion

                string applicationNo = txtappNo.Text.Trim().ToUpper();
                string insPersonSalutation = string.Empty;
                string insPersonPAN = string.Empty;
                string insPersonMobile = string.Empty;
                string insPersonPincode = string.Empty;
                string nomineeDOB = string.Empty;
                string tenor = string.Empty;
                string stateName = string.Empty;
                string cutomerType = string.Empty;

                string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);

                //To get insured person mobile,PAN,Salutation,State and nominee DOB
                string query = @"select C.Salutation Salutation, B.pan_number__c PAN, B.mobile__c Mobile, AC.""name"" Pincode, 
                                NOM.dob__c DOB, A.tenor_in_months__c Tenor,B.residence_state__c state, B.customer_type__c CustomerType
                                from sf_cas_lms_fsprod.application A
                                left join sf_cas_lms_fsprod.sf_loan_applicant B on A.Insured_Person__c = B.id
                                left join sf_cas_lms_fsprod.sf_account C on B.customer_information__c = C.id
                                left join sf_cas_lms_fsprod.sf_ms_pincode AC on B.residence_pincode__c = AC.id
                                left join sf_cas_lms_fsprod.sf_loan_applicant NOM on NOM.id = A.nomineename__c
                                where A.name = '" + applicationNo + "' ";

                Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                builder.TrustServerCertificate = true;
                builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);
                builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]);
                builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                try
                {
                    using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                    {
                        DataTable insPersonData = new DataTable();
                        Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                        da.Fill(insPersonData);
                        DateTime nomiDOB = new DateTime();
                        if (insPersonData.Rows.Count > 0)
                        {
                            insPersonSalutation = insPersonData.Rows[0]["salutation"].ToString();
                            insPersonPAN = insPersonData.Rows[0]["pan"].ToString();
                            insPersonMobile = insPersonData.Rows[0]["mobile"].ToString();
                            insPersonPincode = insPersonData.Rows[0]["pincode"].ToString();
                            if (!string.IsNullOrEmpty(insPersonData.Rows[0]["dob"].ToString()))
                            {
                                nomiDOB = DateTime.Parse(insPersonData.Rows[0]["dob"].ToString());
                                if (nomiDOB != null)
                                {
                                    nomineeDOB = nomiDOB.ToString("dd/MM/yyyy");
                                }
                            }
                            tenor = insPersonData.Rows[0]["tenor"].ToString();
                            stateName = insPersonData.Rows[0]["state"].ToString();
                            cutomerType = insPersonData.Rows[0]["customertype"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                    throw ex;
                }

                //Default Data
                string masterPolicyHolderName = "Five Star Business Finance Limited";
                string masterPolicyHolderPolicyNo = "PP000470";
                string mainBenefit = "decreasing";
                string loanType = "Business";
                string countryOfResidence = "India";
                string nomineeShare = "100%";

                string appNo = txtappNo.Text.Trim(); //dt.Rows[0]["Loan Account No"].ToString();

                string CIFID = dt.Rows[0]["Neo CIF ID"].ToString(); //string CIFID = dt.Rows[0]["Neo Customer ID"].ToString();

                if (Convert.ToString(dt.Rows[0]["IP Name"]) != "")
                {
                    string IPName = dt.Rows[0]["IP Name"].ToString();
                    if (Convert.ToString(dt.Rows[0]["NP Name"]) != "")
                    {
                        string NPName = dt.Rows[0]["NP Name"].ToString();
                        string BranchName = dt.Rows[0]["Branch Name"].ToString();

                        string DOB = string.Empty;
                        if (!string.IsNullOrEmpty(dt.Rows[0]["Date Of Birth"].ToString()))
                        {
                            DOB = Convert.ToDateTime(dt.Rows[0]["Date Of Birth"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }
                        string Address = dt.Rows[0]["Address"].ToString();

                        string RelationShip = dt.Rows[0]["Nominee Relationship"].ToString();
                        string SanctionAmount = dt.Rows[0]["Sanction Loan Amount"].ToString();
                        string Gender = dt.Rows[0]["Gender"].ToString();


                        string page1Path = Server.MapPath(@"~\Files\DOGH_HDFC_P1.jpg");
                        string page2Path = Server.MapPath(@"~\Files\DOGH_HDFC_P2.jpg");
                        string Tick = Server.MapPath(@"~\Images\tick1.png");

                        Bitmap b = new Bitmap(page1Path);
                        Graphics g = Graphics.FromImage(b);

                        Bitmap bitmapP2 = new Bitmap(page2Path);

                        var date = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        System.Drawing.Font f = new System.Drawing.Font("Arial", 4, FontStyle.Regular);
                        System.Drawing.Font S_f = new System.Drawing.Font("Arial", 6, FontStyle.Bold);

                        //tenor
                        //Payment Term (months)
                        if (!string.IsNullOrEmpty(tenor))
                        {
                            g.DrawString(tenor.ToString().ToString(), S_f, SystemBrushes.WindowText, new Point(1720, 460));

                            //Term (months)
                            g.DrawString(tenor.ToString().ToString(), S_f, SystemBrushes.WindowText, new Point(2160, 460));

                            //Loan Term (months)
                            if (!string.IsNullOrEmpty(tenor))
                            {
                                int tCount = 0;
                                foreach (var t in tenor)
                                {
                                    if (tCount == 1)
                                    {
                                        g.DrawString(t.ToString().ToString(), S_f, SystemBrushes.WindowText, new Point(2258, 675));
                                        break;
                                    }
                                    g.DrawString(t.ToString().ToString(), S_f, SystemBrushes.WindowText, new Point(2199, 675));
                                    tCount++;
                                }
                            }
                        }

                        //Options
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 347, 331, 20, 20);

                        //Master Policy Holder Name
                        if (!string.IsNullOrEmpty(masterPolicyHolderName))
                            g.DrawString(masterPolicyHolderName.ToString(), S_f, SystemBrushes.WindowText, new Point(425, 556));

                        //Master Policy Holder Policy No
                        if (!string.IsNullOrEmpty(masterPolicyHolderPolicyNo))
                            g.DrawString(masterPolicyHolderPolicyNo.ToString(), S_f, SystemBrushes.WindowText, new Point(1262, 556));

                        //Application Status
                        if (cutomerType == "Primary Applicant")
                        {
                            g.DrawImage(System.Drawing.Image.FromFile(Tick), 1759, 575, 20, 20);
                        }
                        if (cutomerType == "Co-Applicant")
                        {
                            g.DrawImage(System.Drawing.Image.FromFile(Tick), 2034, 575, 20, 20);
                        }

                        //Moratorium Period (month)
                        g.DrawString("N", S_f, SystemBrushes.WindowText, new Point(434, 620));
                        g.DrawString("A", S_f, SystemBrushes.WindowText, new Point(494, 620));

                        //Main benefit
                        if (!string.IsNullOrEmpty(mainBenefit))
                            g.DrawString(mainBenefit.ToString(), S_f, SystemBrushes.WindowText, new Point(1000, 612));

                        //Loan type
                        if (!string.IsNullOrEmpty(loanType))
                            g.DrawString(loanType.ToString(), S_f, SystemBrushes.WindowText, new Point(243, 670));

                        //Loan Amount
                        if (!string.IsNullOrEmpty(SanctionAmount))
                            g.DrawString(SanctionAmount.ToString(), S_f, SystemBrushes.WindowText, new Point(1145, 670));

                        //Sanction Amount
                        if (!string.IsNullOrEmpty(SanctionAmount))
                            g.DrawString(SanctionAmount.ToString(), S_f, SystemBrushes.WindowText, new Point(350, 460));

                        // IP Salutation
                        if (!string.IsNullOrEmpty(insPersonSalutation))
                        {

                            if (insPersonSalutation == "Mr" || insPersonSalutation == "Mr.")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 116, 1006, 20, 20);
                            }
                            if (insPersonSalutation == "Mrs" || insPersonSalutation == "Mrs.")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 208, 1006, 20, 20);
                            }
                            if (insPersonSalutation == "Ms" || insPersonSalutation == "Ms.")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 315, 1006, 20, 20);
                            }
                            if (insPersonSalutation == "Dr" || insPersonSalutation == "Dr.")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 415, 1006, 20, 20);
                            }

                        }

                        // IP Name
                        if (!string.IsNullOrEmpty(IPName))
                        {
                            int nameLeftPoint = 200;
                            int nameLeftPointLength = 0;
                            foreach (var i in IPName)
                            {
                                g.DrawString(i.ToString(), S_f, SystemBrushes.WindowText, new Point(nameLeftPoint, 1060));
                                nameLeftPoint = nameLeftPoints[nameLeftPointLength + 1];
                                nameLeftPointLength++;
                            }
                        }


                        // DOB
                        if (!string.IsNullOrEmpty(DOB))
                        {
                            int dobLeftPoint = 310;
                            int dobCount = 0;
                            string dob = DOB.Replace("/", "").ToString();
                            foreach (var d in dob)
                            {
                                g.DrawString(d.ToString(), S_f, SystemBrushes.WindowText, new Point(dobLeftPoint, 1130));
                                if (dobCount < 7)
                                {
                                    dobLeftPoint = dobLeftPoints[dobCount + 1];
                                    dobCount++;
                                }
                            }
                        }

                        #region address print
                        //// Address
                        //if (!string.IsNullOrEmpty(Address))
                        //{
                        //    int addrR1Count = 0;
                        //    int addrR2Count = 0;
                        //    int addrRow1Point = 480;
                        //    int addrRow2Point = 200;
                        //    string address1 = string.Empty;
                        //    string address2 = string.Empty;
                        //    if (Address.Length > 34)
                        //    {
                        //        address1 = Address.Substring(0, 34);
                        //        address2 = Address.Substring(34);
                        //    }
                        //    foreach (var addr1 in address1)
                        //    {
                        //        if (addrR1Count <= 33)
                        //        {
                        //            g.DrawString(addr1.ToString(), S_f, SystemBrushes.WindowText, new Point(addrRow1Point, 1260));
                        //            addrRow1Point = addrR1Count < 33 ? addrRow1LeftPoints[addrR1Count + 1] : 34;
                        //            addrR1Count++;
                        //        }
                        //    }
                        //    foreach (var addr2 in address2)
                        //    {
                        //        if (addrR2Count == 39)
                        //        {
                        //            continue;
                        //        }
                        //        g.DrawString(addr2.ToString(), S_f, SystemBrushes.WindowText, new Point(addrRow2Point, 1316));
                        //        addrRow2Point = addrR2Count < 38 ? addrRow2LeftPoints[addrR2Count + 1] : 39;
                        //        addrR2Count++;
                        //    }
                        //}
                        #endregion

                        // Gender
                        if (!string.IsNullOrEmpty(Gender))
                        {
                            if (Gender == "Male")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 910, 1139, 20, 20);
                            }
                            if (Gender == "Female")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 1010, 1139, 20, 20);
                            }
                            if (Gender == "Transgender")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 1233, 1139, 20, 20);
                            }
                        }

                        //IP PAN Number

                        if (!string.IsNullOrEmpty(insPersonPAN))
                        {
                            int PANLeftPoint = 1590;
                            int panCount = 0;
                            foreach (var pan in insPersonPAN)
                            {
                                g.DrawString(pan.ToString(), S_f, SystemBrushes.WindowText, new Point(PANLeftPoint, 1135));
                                if (panCount < 9)
                                {
                                    PANLeftPoint = ipPANLeftPoints[panCount + 1];
                                    panCount++;
                                }
                            }
                        }

                        //IP State
                        if (!string.IsNullOrEmpty(stateName))
                        {
                            int stateLeftPoint = 985;
                            int stateCount = 0;
                            foreach (var st in stateName)
                            {
                                if (st.ToString() == " " || st.ToString() == "")
                                {
                                    continue;
                                }
                                g.DrawString(st.ToString(), S_f, SystemBrushes.WindowText, new Point(stateLeftPoint, 1380));
                                if (stateCount < 15)
                                {
                                    stateLeftPoint = ipStateLeftPoints[stateCount + 1];
                                    stateCount++;
                                }
                            }
                        }

                        //IP Pincode
                        if (!string.IsNullOrEmpty(insPersonPincode))
                        {
                            int pincodeLeftPoint = 2042;
                            int pincodeCount = 0;
                            foreach (var pc in insPersonPincode)
                            {
                                if (pc.ToString() == " " || pc.ToString() == "")
                                {
                                    continue;
                                }
                                g.DrawString(pc.ToString(), S_f, SystemBrushes.WindowText, new Point(pincodeLeftPoint, 1380));
                                if (pincodeCount < 5)
                                {
                                    pincodeLeftPoint = ipPincodeLeftPoints[pincodeCount + 1];
                                    pincodeCount++;
                                }
                            }
                        }


                        //IP Mobile number
                        if (!string.IsNullOrEmpty(insPersonMobile))
                        {
                            int mobileLeftPoint = 1295;
                            int mobileCount = 0;
                            foreach (var mn in insPersonMobile)
                            {
                                if (mn.ToString() == " " || mn.ToString() == "")
                                {
                                    continue;
                                }
                                g.DrawString(mn.ToString(), S_f, SystemBrushes.WindowText, new Point(mobileLeftPoint, 1505));
                                if (mobileCount < 9)
                                {
                                    mobileLeftPoint = ipMobileLeftPoints[mobileCount + 1];
                                    mobileCount++;
                                }
                            }
                        }

                        // Country of Residence
                        if (!string.IsNullOrEmpty(countryOfResidence))
                            g.DrawString(countryOfResidence, S_f, SystemBrushes.WindowText, new Point(360, 1505));

                        //Nationality
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 260, 1450, 20, 20);

                        //Residence
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 913, 1450, 20, 20);

                        //Nominee details
                        // Name
                        if (!string.IsNullOrEmpty(NPName))
                            g.DrawString(NPName, S_f, SystemBrushes.WindowText, new Point(235, 2065));

                        // DOB
                        if (!string.IsNullOrEmpty(nomineeDOB))
                            g.DrawString(nomineeDOB, S_f, SystemBrushes.WindowText, new Point(840, 2065));

                        // Relationship
                        if (!string.IsNullOrEmpty(RelationShip))
                            g.DrawString(RelationShip, S_f, SystemBrushes.WindowText, new Point(1130, 2065));

                        //Share
                        if (!string.IsNullOrEmpty(nomineeShare))
                            g.DrawString(nomineeShare, S_f, SystemBrushes.WindowText, new Point(1857, 2065));

                        // Create PDF document
                        Document document = new Document();
                        MemoryStream stream = new MemoryStream();
                        PdfWriter.GetInstance(document, stream);

                        document.Open();

                        AddImageToPdf(document, b);
                        document.NewPage();
                        AddImageToPdf(document, bitmapP2);

                        document.Close();

                        string fileName = $"{appNo}_DOGH_HDFC.pdf";
                        HttpContext.Current.Response.ContentType = "application/pdf";
                        HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                        HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();

                        document.Dispose();
                        AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
                    }
                    else
                    {
                        _errorMessage = Operations_Messages.ERR_MSG_NOMINEE_NOT_FOUND;
                        AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);

                        return;
                    }
                }
                else
                {
                    _errorMessage = Operations_Messages.ERR_MSG_INSUR_PER_NOT_FOUND;
                    AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);
                    return;
                }

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }

        }
        protected void GenerateHDFCNewForm(DataTable dt, DataTable dtBorrowerCoName, DataTable dtMaritalstatus, DataTable dtAddress)
        {
            try
            {

                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                #region Box Fields points
                //Name
                int[] nameLeftPoints = { 200, 255, 310, 365, 420, 480, 535, 592, 645, 702, 760, 815, 870, 928, 985, 1035, 1095, 1148, 1205, 1265, 1318, 1375, 1432, 1488, 1545, 1602, 1658, 1712, 1770, 1822, 1880, 1935, 1990, 2047, 2100, 2160, 2217, 2272, 2330 };

                //Date of Birth
                int[] dobLeftPoints = { 310, 365, 420, 475, 533, 588, 645, 698 };

                //Address
                int[] addrRow1LeftPoints = { 476, 536, 586, 646, 701, 756, 816, 871, 926, 981, 1038, 1096, 1148, 1206, 1264, 1318, 1374, 1428, 1491, 1546, 1599, 1654, 1706, 1766, 1824, 1881, 1936, 1986, 2046, 2099, 2156, 2211, 2263, 2319 };

                int[] addrRow2LeftPoints = { 200, 255, 312, 368, 425, 480, 540, 590, 650, 705, 760, 820, 875, 930, 985, 1042, 1100, 1152, 1210, 1268, 1322, 1378, 1432, 1495, 1550, 1603, 1658, 1710, 1770, 1828, 1885, 1940, 1990, 2050, 2103, 2160, 2215, 2267, 2323 };

                //IP Mobile number
                int[] ipMobileLeftPoints = { 1295, 1350, 1410, 1463, 1518, 1577, 1630, 1688, 1745, 1800 };

                //IP State
                int[] ipStateLeftPoints = { 980, 1040, 1095, 1150, 1210, 1265, 1323, 1377, 1432, 1484, 1545, 1597, 1655, 1715, 1768, 1825 };

                //IP PAN
                int[] ipPANLeftPoints = { 1590, 1644, 1700, 1757, 1812, 1870, 1927, 1983, 2033, 2093 };

                //IP Pincode
                int[] ipPincodeLeftPoints = { 2042, 2102, 2160, 2215, 2270, 2326 };

                #endregion

                string applicationNo = txtappNo.Text.Trim().ToUpper();
                string insPersonSalutation = string.Empty;
                string insPersonPAN = string.Empty;
                string insPersonMobile = string.Empty;
                string insPersonPincode = string.Empty;
                string nomineeDOB = string.Empty;
                string tenor = string.Empty;
                string stateName = string.Empty;
                string cutomerType = string.Empty;

                string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);

                //To get insured person mobile,PAN,Salutation,State and nominee DOB
                string query = @"select C.Salutation Salutation, B.pan_number__c PAN, B.mobile__c Mobile, AC.""name"" Pincode, 
                                NOM.dob__c DOB, A.tenor_in_months__c Tenor,B.residence_state__c state, B.customer_type__c CustomerType
                                from sf_cas_lms_fsprod.application A
                                left join sf_cas_lms_fsprod.sf_loan_applicant B on A.Insured_Person__c = B.id
                                left join sf_cas_lms_fsprod.sf_account C on B.customer_information__c = C.id
                                left join sf_cas_lms_fsprod.sf_ms_pincode AC on B.residence_pincode__c = AC.id
                                left join sf_cas_lms_fsprod.sf_loan_applicant NOM on NOM.id = A.nomineename__c
                                where A.name = '" + applicationNo + "' ";

                Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                builder.TrustServerCertificate = true;
                builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);
                builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                //builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]);
                builder.CommandTimeout = 0;
                builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                try
                {
                    using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                    {
                        DataTable insPersonData = new DataTable();
                        Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                        da.Fill(insPersonData);
                        DateTime nomiDOB = new DateTime();
                        if (insPersonData.Rows.Count > 0)
                        {
                            insPersonSalutation = insPersonData.Rows[0]["salutation"].ToString();
                            insPersonPAN = insPersonData.Rows[0]["pan"].ToString();
                            insPersonMobile = insPersonData.Rows[0]["mobile"].ToString();
                            insPersonPincode = insPersonData.Rows[0]["pincode"].ToString();
                            if (!string.IsNullOrEmpty(insPersonData.Rows[0]["dob"].ToString()))
                            {
                                nomiDOB = DateTime.Parse(insPersonData.Rows[0]["dob"].ToString());
                                if (nomiDOB != null)
                                {
                                    nomineeDOB = nomiDOB.ToString("dd/MM/yyyy");
                                }
                            }
                            tenor = insPersonData.Rows[0]["tenor"].ToString();
                            stateName = insPersonData.Rows[0]["state"].ToString();
                            cutomerType = insPersonData.Rows[0]["customertype"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                    throw ex;
                }

                //Default Data
                string masterPolicyHolderName = "Five Star Business Finance Limited";
                string masterPolicyHolderPolicyNo = "PP000470";
                string mainBenefit = "decreasing";
                string loanType = "Business";
                string countryOfResidence = "India";
                string nomineeShare = "100%";

                string appNo = txtappNo.Text.Trim(); //dt.Rows[0]["Loan Account No"].ToString();

                string CIFID = dt.Rows[0]["Neo CIF ID"].ToString(); //string CIFID = dt.Rows[0]["Neo Customer ID"].ToString();

                if (Convert.ToString(dt.Rows[0]["IP Name"]) != "")
                {
                    string IPName = dt.Rows[0]["IP Name"].ToString();
                    if (Convert.ToString(dt.Rows[0]["NP Name"]) != "")
                    {
                        string NPName = dt.Rows[0]["NP Name"].ToString();
                        string BranchName = dt.Rows[0]["Branch Name"].ToString();

                        string DOB = string.Empty;
                        if (!string.IsNullOrEmpty(dt.Rows[0]["Date Of Birth"].ToString()))
                        {
                            DOB = Convert.ToDateTime(dt.Rows[0]["Date Of Birth"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        string RelationShip = dt.Rows[0]["Nominee Relationship"].ToString();
                        string SanctionAmount = dt.Rows[0]["Sanction Loan Amount"].ToString();
                        string Gender = dt.Rows[0]["Gender"].ToString();


                        //string page1Path = Server.MapPath(@"~\Files\DOGH_NEW_HDFC_P1.jpg");
                        //string page2Path = Server.MapPath(@"~\Files\DOGH_NEW_HDFC_P2.jpg");
                        string page1Path = Server.MapPath(ConfigurationManager.AppSettings["HDFCPage1"].ToString());
                        string page2Path = Server.MapPath(ConfigurationManager.AppSettings["HDFCPage2"].ToString());
                        string Tick = Server.MapPath(ConfigurationManager.AppSettings["HDFCPageTick"].ToString());

                        Bitmap b = new Bitmap(page1Path);
                        Graphics g = Graphics.FromImage(b);

                        Bitmap bitmapP2 = new Bitmap(page2Path);

                        var date = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        System.Drawing.Font f = new System.Drawing.Font("Arial", 4, FontStyle.Regular);
                        System.Drawing.Font S_f = new System.Drawing.Font("Arial", 6, FontStyle.Bold);

                        //tenor
                        //Payment Term (months)
                        if (!string.IsNullOrEmpty(tenor))
                        {
                            g.DrawString(tenor.ToString().ToString(), S_f, SystemBrushes.WindowText, new Point(1750, 420));

                            //Term (months)
                            g.DrawString(tenor.ToString().ToString(), S_f, SystemBrushes.WindowText, new Point(2160, 420));

                            //Loan Term (months)
                            if (!string.IsNullOrEmpty(tenor))
                            {
                                int tCount = 0;
                                foreach (var t in tenor)
                                {
                                    if (tCount == 1)
                                    {
                                        g.DrawString(t.ToString().ToString(), S_f, SystemBrushes.WindowText, new Point(2252, 615));
                                        break;
                                    }
                                    g.DrawString(t.ToString().ToString(), S_f, SystemBrushes.WindowText, new Point(2197, 615));
                                    tCount++;
                                }
                            }
                        }

                        //Options
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 347, 290, 20, 20);

                        //Master Policy Holder Name
                        if (!string.IsNullOrEmpty(masterPolicyHolderName))
                            g.DrawString(masterPolicyHolderName.ToString(), S_f, SystemBrushes.WindowText, new Point(425, 495));

                        //Master Policy Holder Policy No
                        if (!string.IsNullOrEmpty(masterPolicyHolderPolicyNo))
                            g.DrawString(masterPolicyHolderPolicyNo.ToString(), S_f, SystemBrushes.WindowText, new Point(1262, 495));

                        string sBorrowerName = string.Empty;
                        string sCoBorrowerName = string.Empty;
                        if (dtBorrowerCoName != null && dtBorrowerCoName.Rows.Count > 0)
                        {
                            sBorrowerName = string.IsNullOrEmpty(dtBorrowerCoName.Rows[0]["CUSTOMER_NAME"].ToString()) ? "" : dtBorrowerCoName.Rows[0]["CUSTOMER_NAME"].ToString();
                            sCoBorrowerName = string.IsNullOrEmpty(dtBorrowerCoName.Rows[0]["COBORROWER_FULL_NAME"].ToString()) ? "" : dtBorrowerCoName.Rows[0]["COBORROWER_FULL_NAME"].ToString();
                        }

                        //Application Status 
                        if (cutomerType == "Primary Applicant")
                        {
                            g.DrawImage(System.Drawing.Image.FromFile(Tick), 1759, 513, 20, 20);
                            g.DrawString(sBorrowerName, S_f, SystemBrushes.WindowText, new PointF(440, 900));
                        }
                        if (cutomerType == "Co-Applicant")
                        {
                            g.DrawImage(System.Drawing.Image.FromFile(Tick), 2034, 513, 20, 20);
                            g.DrawString(sCoBorrowerName, S_f, SystemBrushes.WindowText, new PointF(1500, 900));
                        }

                        //Type of cover
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 286, 822, 20, 20);

                        // MARITAL STATUS
                        if(dtMaritalstatus != null && dtMaritalstatus.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(dtMaritalstatus.Rows[0]["MARITAL_STATUS_CODE"].ToString()))
                            {
                                if (dtMaritalstatus.Rows[0]["MARITAL_STATUS_CODE"].ToString().ToLower() == "married")
                                {
                                    g.DrawImage(System.Drawing.Image.FromFile(Tick), 664, 1370, 20, 20);
                                }
                                if (dtMaritalstatus.Rows[0]["MARITAL_STATUS_CODE"].ToString().ToLower() == "unmarried")
                                {
                                    g.DrawImage(System.Drawing.Image.FromFile(Tick), 884, 1370, 20, 20);
                                }
                                if (dtMaritalstatus.Rows[0]["MARITAL_STATUS_CODE"].ToString().ToLower() == "widower")
                                {
                                    g.DrawImage(System.Drawing.Image.FromFile(Tick), 1140, 1370, 20, 20);
                                }
                                if (dtMaritalstatus.Rows[0]["MARITAL_STATUS_CODE"].ToString().ToLower() == "single")
                                {
                                    g.DrawImage(System.Drawing.Image.FromFile(Tick), 1365, 1370, 20, 20);
                                }
                            }
                        }
                        

                        //Moratorium Period (month)
                        g.DrawString("N", S_f, SystemBrushes.WindowText, new Point(434, 560));
                        g.DrawString("A", S_f, SystemBrushes.WindowText, new Point(494, 560));

                        //Main benefit
                        if (!string.IsNullOrEmpty(mainBenefit))
                            g.DrawString(mainBenefit.ToString(), S_f, SystemBrushes.WindowText, new Point(1000, 557));

                        //Loan type
                        if (!string.IsNullOrEmpty(loanType))
                            g.DrawString(loanType.ToString(), S_f, SystemBrushes.WindowText, new Point(243, 615));

                        //Loan Amount
                        if (!string.IsNullOrEmpty(SanctionAmount))
                            g.DrawString(SanctionAmount.ToString(), S_f, SystemBrushes.WindowText, new Point(1124, 615));

                        //Sanction Amount
                        if (!string.IsNullOrEmpty(SanctionAmount))
                            g.DrawString(SanctionAmount.ToString(), S_f, SystemBrushes.WindowText, new Point(460, 420));

                        // IP Salutation
                        if (!string.IsNullOrEmpty(insPersonSalutation))
                        {

                            if (insPersonSalutation == "Mr" || insPersonSalutation == "Mr.")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 116, 1179, 20, 20);
                            }
                            if (insPersonSalutation == "Mrs" || insPersonSalutation == "Mrs.")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 208, 1179, 20, 20);
                            }
                            if (insPersonSalutation == "Ms" || insPersonSalutation == "Ms.")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 315, 1179, 20, 20);
                            }
                            if (insPersonSalutation == "Dr" || insPersonSalutation == "Dr.")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 415, 1179, 20, 20);
                            }

                        }

                        

                        // IP Name
                        if (!string.IsNullOrEmpty(IPName))
                        {
                            int nameLeftPoint = 200;
                            int nameLeftPointLength = 0;
                            foreach (var i in IPName)
                            {
                                g.DrawString(i.ToString(), S_f, SystemBrushes.WindowText, new Point(nameLeftPoint, 1241));
                                if (nameLeftPointLength < 38)
                                {
                                    nameLeftPoint = nameLeftPoints[nameLeftPointLength + 1];
                                    nameLeftPointLength++;
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }


                        // DOB
                        if (!string.IsNullOrEmpty(DOB))
                        {
                            int dobLeftPoint = 310;
                            int dobCount = 0;
                            string dob = DOB.Replace("/", "").ToString();
                            foreach (var d in dob)
                            {
                                g.DrawString(d.ToString(), S_f, SystemBrushes.WindowText, new Point(dobLeftPoint, 1306));
                                if (dobCount < 7)
                                {
                                    dobLeftPoint = dobLeftPoints[dobCount + 1];
                                    dobCount++;
                                }
                            }
                        }

                        #region address print
                        // Address
                        if (dtAddress != null && dtAddress.Rows.Count > 0)
                        {
                            string Address = dtAddress.Rows[0]["Address"].ToString();
                            if (!string.IsNullOrEmpty(Address))
                            {
                                int addrR1Count = 0;
                                int addrR2Count = 0;
                                int addrRow1Point = 480;
                                int addrRow2Point = 200;
                                string address1 = string.Empty;
                                string address2 = string.Empty;
                                if (Address.Length <= 73)
                                {
                                    if (Address.Length > 34)
                                    {
                                        address1 = Address.Substring(0, 34);
                                        address2 = Address.Substring(34);
                                    }
                                    else
                                    {
                                        address1 = Address;
                                    }
                                    foreach (var addr1 in address1)
                                    {
                                        if (addrR1Count <= 33)
                                        {
                                            g.DrawString(addr1.ToString(), S_f, SystemBrushes.WindowText, new Point(addrRow1Point, 1440));
                                            addrRow1Point = addrR1Count < 33 ? addrRow1LeftPoints[addrR1Count + 1] : 34;
                                            addrR1Count++;
                                        }
                                    }
                                    foreach (var addr2 in address2)
                                    {
                                        if (addrR2Count == 39)
                                        {
                                            continue;
                                        }
                                        g.DrawString(addr2.ToString(), S_f, SystemBrushes.WindowText, new Point(addrRow2Point, 1496));
                                        addrRow2Point = addrR2Count < 38 ? addrRow2LeftPoints[addrR2Count + 1] : 39;
                                        addrR2Count++;
                                    }
                                }

                            }
                        }
                        #endregion

                        // Gender
                        if (!string.IsNullOrEmpty(Gender))
                        {
                            if (Gender == "Male")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 910, 1303, 20, 20);
                            }
                            if (Gender == "Female")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 1010, 1303, 20, 20);
                            }
                            if (Gender == "Transgender")
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 1233, 1303, 20, 20);
                            }
                        }

                        //IP PAN Number

                        if (!string.IsNullOrEmpty(insPersonPAN))
                        {
                            int PANLeftPoint = 1590;
                            int panCount = 0;
                            foreach (var pan in insPersonPAN)
                            {
                                g.DrawString(pan.ToString(), S_f, SystemBrushes.WindowText, new Point(PANLeftPoint, 1306));
                                if (panCount < 9)
                                {
                                    PANLeftPoint = ipPANLeftPoints[panCount + 1];
                                    panCount++;
                                }
                            }
                        }

                        //IP State
                        if (!string.IsNullOrEmpty(stateName))
                        {
                            int stateLeftPoint = 985;
                            int stateCount = 0;
                            foreach (var st in stateName)
                            {
                                if (st.ToString() == " " || st.ToString() == "")
                                {
                                    continue;
                                }
                                g.DrawString(st.ToString(), S_f, SystemBrushes.WindowText, new Point(stateLeftPoint, 1560));
                                if (stateCount < 15)
                                {
                                    stateLeftPoint = ipStateLeftPoints[stateCount + 1];
                                    stateCount++;
                                }
                            }
                        }

                        //IP Pincode
                        if (!string.IsNullOrEmpty(insPersonPincode))
                        {
                            int pincodeLeftPoint = 2042;
                            int pincodeCount = 0;
                            foreach (var pc in insPersonPincode)
                            {
                                if (pc.ToString() == " " || pc.ToString() == "")
                                {
                                    continue;
                                }
                                g.DrawString(pc.ToString(), S_f, SystemBrushes.WindowText, new Point(pincodeLeftPoint, 1560));
                                if (pincodeCount < 5)
                                {
                                    pincodeLeftPoint = ipPincodeLeftPoints[pincodeCount + 1];
                                    pincodeCount++;
                                }
                            }
                        }


                        //IP Mobile number
                        if (!string.IsNullOrEmpty(insPersonMobile))
                        {
                            int mobileLeftPoint = 1295;
                            int mobileCount = 0;
                            foreach (var mn in insPersonMobile)
                            {
                                if (mn.ToString() == " " || mn.ToString() == "")
                                {
                                    continue;
                                }
                                g.DrawString(mn.ToString(), S_f, SystemBrushes.WindowText, new Point(mobileLeftPoint, 1687));
                                if (mobileCount < 9)
                                {
                                    mobileLeftPoint = ipMobileLeftPoints[mobileCount + 1];
                                    mobileCount++;
                                }
                            }
                        }

                        // Country of Residence
                        if (!string.IsNullOrEmpty(countryOfResidence))
                            g.DrawString(countryOfResidence, S_f, SystemBrushes.WindowText, new Point(360, 1691));

                        //Nationality
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 260, 1633, 20, 20);

                        //Residence
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 913, 1633, 20, 20);

                        //Nominee details
                        // Name
                        if (!string.IsNullOrEmpty(NPName))
                            g.DrawString(NPName, S_f, SystemBrushes.WindowText, new Point(235, 2185));

                        // DOB
                        if (!string.IsNullOrEmpty(nomineeDOB))
                            g.DrawString(nomineeDOB, S_f, SystemBrushes.WindowText, new Point(840, 2185));

                        // Relationship
                        if (!string.IsNullOrEmpty(RelationShip))
                            g.DrawString(RelationShip, S_f, SystemBrushes.WindowText, new Point(1130, 2185));

                        //Share
                        if (!string.IsNullOrEmpty(nomineeShare))
                            g.DrawString(nomineeShare, S_f, SystemBrushes.WindowText, new Point(1857, 2185));

                        // Create PDF document
                        Document document = new Document();
                        MemoryStream stream = new MemoryStream();
                        PdfWriter.GetInstance(document, stream);

                        document.Open();

                        AddImageToPdf(document, b);
                        document.NewPage();
                        AddImageToPdf(document, bitmapP2);

                        document.Close();

                        string fileName = $"{appNo}_DOGH_HDFC.pdf";
                        HttpContext.Current.Response.ContentType = "application/pdf";
                        HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                        HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();

                        document.Dispose();
                        AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
                    }
                    else
                    {
                        _errorMessage = Operations_Messages.ERR_MSG_NOMINEE_NOT_FOUND;
                        AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);

                        return;
                    }
                }
                else
                {
                    _errorMessage = Operations_Messages.ERR_MSG_INSUR_PER_NOT_FOUND;
                    AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);
                    return;
                }

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }

        }
        protected void GenerateGoDigitForm(DataTable dt)
        {
            try
            {

                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                string applicationNo = txtappNo.Text.Trim().ToUpper();
                string nomineeDOB = string.Empty;
                string tenor = string.Empty;
                string stateName = string.Empty;

                string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);

                //To get insured person mobile,PAN,Salutation,State and nominee DOB
                string query = @"select C.Salutation Salutation, B.pan_number__c PAN, B.mobile__c Mobile, AC.""name"" Pincode, 
                                NOM.dob__c DOB, A.tenor_in_months__c Tenor,B.residence_state__c state, B.customer_type__c CustomerType
                                from sf_cas_lms_fsprod.application A
                                left join sf_cas_lms_fsprod.sf_loan_applicant B on A.Insured_Person__c = B.id
                                left join sf_cas_lms_fsprod.sf_account C on B.customer_information__c = C.id
                                left join sf_cas_lms_fsprod.sf_ms_pincode AC on B.residence_pincode__c = AC.id
                                left join sf_cas_lms_fsprod.sf_loan_applicant NOM on NOM.id = A.nomineename__c
                                where A.name = '" + applicationNo + "' ";

                Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                builder.TrustServerCertificate = true;
                builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);
                builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                //builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]);
                builder.CommandTimeout = 0;
                builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                try
                {
                    using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                    {
                        DataTable insPersonData = new DataTable();
                        Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                        da.Fill(insPersonData);
                        DateTime nomiDOB = new DateTime();
                        if (insPersonData.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(insPersonData.Rows[0]["dob"].ToString()))
                            {
                                nomiDOB = DateTime.Parse(insPersonData.Rows[0]["dob"].ToString());
                                if (nomiDOB != null)
                                {
                                    nomineeDOB = nomiDOB.ToString("dd/MM/yyyy");
                                }
                            }
                            tenor = insPersonData.Rows[0]["tenor"].ToString();
                            stateName = insPersonData.Rows[0]["state"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                    throw ex;
                }

                string appNo = txtappNo.Text.Trim();
                string LoanAccount = dt.Rows[0]["Loan Account No"].ToString();
                string IPName = dt.Rows[0]["IP Name"].ToString();
                string IPDOB = string.Empty;
                string NPName = dt.Rows[0]["NP Name"].ToString();
                string SanctionAmount = dt.Rows[0]["Sanction Loan Amount"].ToString();
                string Gender = dt.Rows[0]["Gender"].ToString();
                string PremiumAmount = dt.Rows[0]["Premium_Amount"].ToString();

                if (Convert.ToString(dt.Rows[0]["IP Name"]) != "")
                {
                    if (Convert.ToString(dt.Rows[0]["NP Name"]) != "")
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0]["Date Of Birth"].ToString()))
                        {
                            IPDOB = Convert.ToDateTime(dt.Rows[0]["Date Of Birth"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        string page1Path = Server.MapPath(ConfigurationManager.AppSettings["GODIGITP1"].ToString());
                        string page2Path = Server.MapPath(ConfigurationManager.AppSettings["GODIGITP2"].ToString());

                        Bitmap b = new Bitmap(page1Path);
                        Graphics g = Graphics.FromImage(b);

                        Bitmap bitmapP2 = new Bitmap(page2Path);

                        var date = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        System.Drawing.Font f = new System.Drawing.Font("Arial", 4, FontStyle.Regular);
                        System.Drawing.Font M_f = new System.Drawing.Font("Arial", 9, FontStyle.Regular);
                        System.Drawing.Font S_f = new System.Drawing.Font("Arial", 6, FontStyle.Bold);

                        //Loan Account Number 
                        g.DrawString(LoanAccount, S_f, SystemBrushes.WindowText, new Point(835, 490));

                        //Name of the Applicant
                        string ipName35 = IPName.Length >= 35 ? IPName.Substring(0, 35) : IPName;
                        g.DrawString(ipName35, S_f, SystemBrushes.WindowText, new Point(835, 540));

                        //Name of the Applicant  DOB
                        if (!string.IsNullOrEmpty(IPDOB))
                        {
                            IPDOB = GetFormatedDate(IPDOB);
                            g.DrawString(IPDOB, S_f, SystemBrushes.WindowText, new Point(1643, 540));
                        }

                        //Applicant  Gender
                        if (!string.IsNullOrEmpty(Gender))
                        {
                            g.DrawString(Gender, S_f, SystemBrushes.WindowText, new Point(2060, 540));
                        }

                        //Nominee Name
                        string npName53 = NPName.Length >= 35 ? NPName.Substring(0, 35) : NPName;
                        g.DrawString(npName53, S_f, SystemBrushes.WindowText, new Point(835, 590));

                        //Nominee DOB
                        if (!string.IsNullOrEmpty(nomineeDOB))
                        {
                            nomineeDOB = GetFormatedDate(nomineeDOB);
                            g.DrawString(nomineeDOB, S_f, SystemBrushes.WindowText, new Point(1645, 590));
                        }
                        //Sum Assured
                        if (!string.IsNullOrEmpty(SanctionAmount))
                        {
                            string formattedAmount = GetFormatedAmount(SanctionAmount);
                            g.DrawString(formattedAmount, M_f, SystemBrushes.WindowText, new Point(928, 1009));
                        }

                        //Cover Term
                        if (!string.IsNullOrEmpty(tenor))
                        {
                            g.DrawString(tenor, M_f, SystemBrushes.WindowText, new Point(928, 1057));
                        }

                        //Premium Amount
                        //if (!string.IsNullOrEmpty(PremiumAmount))
                        //{
                        //    string formattedAmount = GetFormatedAmount(PremiumAmount);
                        //    g.DrawString(formattedAmount, M_f, SystemBrushes.WindowText, new Point(928, 1105));
                        //}

                        //l/We am aware that
                        if (!string.IsNullOrEmpty(IPName))
                        {
                            string ipName53 = IPName.Length >= 53 ? IPName.Substring(0, 53) : IPName;
                            g.DrawString(ipName53, S_f, SystemBrushes.WindowText, new Point(556, 1922));
                        }

                        //Name
                        string ipName70 = IPName.Length >= 70 ? IPName.Substring(0, 70) : IPName;
                        g.DrawString(ipName70, S_f, SystemBrushes.WindowText, new Point(355, 3284));

                        // Create PDF document
                        Document document = new Document();
                        MemoryStream stream = new MemoryStream();
                        PdfWriter.GetInstance(document, stream);

                        document.Open();

                        AddImageToPdf(document, b);
                        document.NewPage();
                        AddImageToPdf(document, bitmapP2);

                        document.Close();

                        string fileName = $"{appNo}_DOGH_GODIGIT.pdf";
                        HttpContext.Current.Response.ContentType = "application/pdf";
                        HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                        HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();

                        document.Dispose();
                        AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
                    }
                    else
                    {
                        _errorMessage = Operations_Messages.ERR_MSG_NOMINEE_NOT_FOUND;
                        AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);

                        return;
                    }
                }
                else
                {
                    _errorMessage = Operations_Messages.ERR_MSG_INSUR_PER_NOT_FOUND;
                    AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);
                    return;
                }

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }

        }

        protected void GenerateMaxLifeIns(DataTable dt, string APPNO)
        {
            try
            {

                _pageName = this.GetType().Name;
                _currMethodName = MethodBase.GetCurrentMethod().Name;
                AuditLogger.RecordDebugStartLog(_pageName, _currMethodName);

                string applicationNo = txtappNo.Text.Trim().ToUpper();
                string nomineeDOB = string.Empty;
                string tenor = string.Empty;
                string stateName = string.Empty;
                string country = string.Empty;
                string panNo = string.Empty;
                string address = string.Empty;
                string mobile = string.Empty;
                string pincode = string.Empty;
                string customerType = string.Empty;
                string ROI = string.Empty;
                string Schema = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_SCHEMA"]);


                //To get insured person mobile,PAN,Salutation,State and nominee DOB
                string query = @"select C.Salutation Salutation, B.PAN_Number__c PAN, B.mobile__c Mobile, AC.""name"" Pincode, 
                                NOM.dob__c DOB, A.tenor_in_months__c Tenor,B.residence_state__c state,B.business_country__c Country, B.customer_type__c CustomerType,A.total_roi_percent__c ROI
                                from sf_cas_lms_fsprod.application A
                                left join sf_cas_lms_fsprod.sf_loan_applicant B on A.Insured_Person__c = B.id
                                left join sf_cas_lms_fsprod.sf_account C on B.customer_information__c = C.id
                                left join sf_cas_lms_fsprod.sf_ms_pincode AC on B.residence_pincode__c = AC.id
                                left join sf_cas_lms_fsprod.sf_loan_applicant NOM on NOM.id = A.nomineename__c
                                where A.name = '" + applicationNo + "' ";


                Npgsql.NpgsqlConnectionStringBuilder builder = new Npgsql.NpgsqlConnectionStringBuilder();
                builder.TrustServerCertificate = true;
                builder.Host = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_HOST"]);
                builder.Port = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_PORT"]);
                builder.Database = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_DATABASE"]);
                builder.Username = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_USERNAME"]);
                builder.Password = Convert.ToString(ConfigurationManager.AppSettings["REDSHIFT_PASSWORD"]);
                //builder.Timeout = Convert.ToInt32(ConfigurationManager.AppSettings["REDSHIFT_TIMEOUT"]);
                builder.CommandTimeout = 0;
                builder.ServerCompatibilityMode = Npgsql.ServerCompatibilityMode.Redshift;

                try
                {
                    using (var conn = new Npgsql.NpgsqlConnection(builder.ConnectionString))
                    {
                        DataTable insPersonData = new DataTable();
                        Npgsql.NpgsqlDataAdapter da = new Npgsql.NpgsqlDataAdapter(query, conn);
                        da.Fill(insPersonData);
                        DateTime nomiDOB = new DateTime();
                        if (insPersonData.Rows.Count > 0)
                        {
                            if (!string.IsNullOrEmpty(insPersonData.Rows[0]["dob"].ToString()))
                            {
                                nomiDOB = DateTime.Parse(insPersonData.Rows[0]["dob"].ToString());
                                if (nomiDOB != null)
                                {
                                    nomineeDOB = nomiDOB.ToString("dd/MM/yyyy");
                                }
                            }
                            tenor = insPersonData.Rows[0]["tenor"].ToString();
                            stateName = insPersonData.Rows[0]["state"].ToString();
                            country = insPersonData.Rows[0]["Country"].ToString();
                            pincode = insPersonData.Rows[0]["Pincode"].ToString();
                            panNo = insPersonData.Rows[0]["PAN"].ToString();
                            mobile = insPersonData.Rows[0]["Mobile"].ToString();
                            customerType = insPersonData.Rows[0]["CustomerType"].ToString();
                            ROI = insPersonData.Rows[0]["ROI"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                    throw ex;
                }




                string appNo = txtappNo.Text.Trim();
                string LoanAccount = dt.Rows[0]["Loan Account No"].ToString();
                string IPName = dt.Rows[0]["IP Name"].ToString();
                string IPDOB = string.Empty;
                string NPName = dt.Rows[0]["NP Name"].ToString();
                string NP_Relation = dt.Rows[0]["nominee relationship"].ToString();
                string SanctionAmount = dt.Rows[0]["Sanction Loan Amount"].ToString();
                string Gender = dt.Rows[0]["Gender"].ToString();
                address = dt.Rows[0]["address"].ToString();
                string PremiumAmount = dt.Rows[0]["Premium_Amount"].ToString();

                string Tick = Server.MapPath(@"~\Images\tick1.png");


                if (Convert.ToString(dt.Rows[0]["IP Name"]) != "")
                {
                    if (Convert.ToString(dt.Rows[0]["NP Name"]) != "")
                    {
                        if (!string.IsNullOrEmpty(dt.Rows[0]["Date Of Birth"].ToString()))
                        {
                            IPDOB = Convert.ToDateTime(dt.Rows[0]["Date Of Birth"].ToString()).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
                        }

                        string page1Path = Server.MapPath(ConfigurationManager.AppSettings["MAXLIFEP1"].ToString());
                        string page2Path = Server.MapPath(ConfigurationManager.AppSettings["MAXLIFEP2"].ToString());

                        Bitmap b = new Bitmap(page1Path);
                        Graphics g = Graphics.FromImage(b);

                        Bitmap bitmapP2 = new Bitmap(page2Path);
                        Graphics g2 = Graphics.FromImage(bitmapP2);

                        var date = DateTime.Now.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);

                        System.Drawing.Font f = new System.Drawing.Font("Arial", 4, FontStyle.Regular);
                        System.Drawing.Font M_f = new System.Drawing.Font("Arial", 9, FontStyle.Regular);
                        System.Drawing.Font S_f = new System.Drawing.Font("Arial", 6, FontStyle.Bold);
                        System.Drawing.Font f_B = new System.Drawing.Font("Arial", 5, FontStyle.Bold);


                        //Loan Account Number 
                        g.DrawString(LoanAccount, S_f, SystemBrushes.WindowText, new Point(683, 640));

                        //Application Number   
                        g.DrawString(appNo, S_f, SystemBrushes.WindowText, new Point(683, 592));

                        //IP Name
                        if (!string.IsNullOrEmpty(IPName) && (IPName.Length <= 41))//IT ALLOWS 41 CHARS
                        {

                            g.DrawString(IPName, S_f, SystemBrushes.WindowText, new Point(573, 1037));
                        }


                        //Name of the Applicant  DOB  MLI
                        if (!string.IsNullOrEmpty(IPDOB))
                        {
                            IPDOB = GetFormatedDate(IPDOB);
                            g.DrawString(IPDOB, S_f, SystemBrushes.WindowText, new Point(1892, 1044));
                        }

                        //ROI
                        if (!string.IsNullOrEmpty(ROI))
                        {

                            // g.DrawString(ROI+"%", S_f, SystemBrushes.WindowText, new Point(1592, 1562));
                            g.DrawString(ROI, S_f, SystemBrushes.WindowText, new Point(1592, 1562));
                        }


                        // customerType
                        if (!string.IsNullOrEmpty(customerType))
                        {
                            if (string.Equals(customerType, "Primary Applicant", StringComparison.OrdinalIgnoreCase))
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 693, 787, 20, 20);
                            }
                            else if (string.Equals(customerType, "Co-Applicant", StringComparison.OrdinalIgnoreCase))
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 975, 787, 20, 20);
                            }
                            else if (string.Equals(customerType, "Guarantor", StringComparison.OrdinalIgnoreCase))
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 1262, 787, 20, 20);
                            }
                            else if (string.Equals(customerType, "Key person", StringComparison.OrdinalIgnoreCase))
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 1539, 787, 20, 20);
                            }
                            else if (string.Equals(customerType, "Trustee", StringComparison.OrdinalIgnoreCase))
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 1822, 790, 20, 20);
                            }
                        }



                        //pan

                        if (!string.IsNullOrEmpty(panNo))
                        {

                            g.DrawString(panNo, S_f, SystemBrushes.WindowText, new Point(402, 1359));
                        }


                        ////Applicant Nominee Name
                        if (!string.IsNullOrEmpty(NPName) && (NPName.Length <= 20)) //IT ALLOWS 13 CHARACTERS
                        {
                            g.DrawString(NPName, S_f, SystemBrushes.WindowText, new Point(345, 2647));

                        }


                        ////Relation
                        if (!string.IsNullOrEmpty(NP_Relation))
                        {
                            g.DrawString(NP_Relation, S_f, SystemBrushes.WindowText, new Point(983, 2647));
                        }
                        //address
                        if (!string.IsNullOrEmpty(address) && (address.Length <= 115))  //ITS ALLOWS 80 CHAR 
                        {

                            g.DrawString(address, S_f, SystemBrushes.WindowText, new Point(577, 2135));
                        }

                        ////city

                        //if (!string.IsNullOrEmpty(city))
                        //{
                        //    g.DrawString(city, S_f, SystemBrushes.WindowText, new Point(669, 2233));  
                        //}

                        //state
                        if (!string.IsNullOrEmpty(stateName))
                        {
                            g.DrawString(stateName, S_f, SystemBrushes.WindowText, new Point(1390, 2183));
                        }

                        //IP mobile number
                        if (!string.IsNullOrEmpty(mobile))
                        {
                            g.DrawString(mobile, S_f, SystemBrushes.WindowText, new Point(1451, 2234));
                        }

                        //pincode
                        if (!string.IsNullOrEmpty(pincode))
                        {
                            IPDOB = GetFormatedDate(pincode);
                            g.DrawString(pincode, S_f, SystemBrushes.WindowText, new Point(1932, 2180));
                        }


                        // Gender
                        if (!string.IsNullOrEmpty(Gender))
                        {
                            if (string.Equals(Gender, "Male", StringComparison.OrdinalIgnoreCase))
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 494, 1103, 20, 20);
                            }
                            else if (string.Equals(Gender, "Female", StringComparison.OrdinalIgnoreCase))
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 732, 1103, 20, 20);
                            }
                            else if (string.Equals(Gender, "Transgender", StringComparison.OrdinalIgnoreCase))
                            {
                                g.DrawImage(System.Drawing.Image.FromFile(Tick), 1047, 1103, 20, 20);
                            }
                        }


                        /// India
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 497, 1158, 20, 20);

                        //Cover type
                        g.DrawImage(System.Drawing.Image.FromFile(Tick), 852, 1780, 20, 20);

                        //% share                        
                        // g.DrawImage(System.Drawing.Image.FromFile(Tick), 1246, 2650);
                        g.DrawString("100%", S_f, SystemBrushes.WindowText, new Point(1246, 2650));



                        //Nominee DOB
                        if (!string.IsNullOrEmpty(nomineeDOB))
                        {
                            nomineeDOB = GetFormatedDate(nomineeDOB);
                            g.DrawString(nomineeDOB, S_f, SystemBrushes.WindowText, new Point(657, 2649));
                        }
                        //Sum Assured
                        if (!string.IsNullOrEmpty(SanctionAmount))
                        {
                            string formattedAmount = GetFormatedAmount(SanctionAmount);
                            g.DrawString(formattedAmount, S_f, SystemBrushes.WindowText, new Point(690, 684));
                        }



                        //tenur
                        if (!string.IsNullOrEmpty(tenor))
                        {
                            g.DrawString(tenor, S_f, SystemBrushes.WindowText, new Point(919, 1565));
                            g.DrawString(tenor, S_f, SystemBrushes.WindowText, new Point(2089, 1927));
                        }

                        //Premium Amount MLI
                        if (!string.IsNullOrEmpty(PremiumAmount))
                        {
                            string formattedAmount = GetFormatedAmount(PremiumAmount);
                            g.DrawString(formattedAmount, S_f, SystemBrushes.WindowText, new Point(870, 1516));
                        }

                        //Authorization field
                        g2.DrawImage(System.Drawing.Image.FromFile(Tick), 215, 1213, 20, 20);

                        g2.DrawString("Five Star Business Finance Ltd.", f_B, SystemBrushes.WindowText, new Point(1556, 1199));




                        // Create PDF document
                        Document document = new Document();
                        MemoryStream stream = new MemoryStream();
                        PdfWriter.GetInstance(document, stream);

                        document.Open();

                        AddImageToPdf(document, b);
                        document.NewPage();
                        AddImageToPdf(document, bitmapP2);

                        document.Close();

                        string fileName = $"{appNo}_DOGH_MAXLIFE.pdf";
                        HttpContext.Current.Response.ContentType = "application/pdf";
                        HttpContext.Current.Response.AppendHeader("Content-Disposition", "attachment; filename=" + fileName);
                        HttpContext.Current.Response.BinaryWrite(stream.ToArray());
                        HttpContext.Current.Response.Flush();
                        HttpContext.Current.Response.End();

                        document.Dispose();
                        AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
                    }
                    else
                    {
                        _errorMessage = Operations_Messages.ERR_MSG_NOMINEE_NOT_FOUND;
                        AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                        ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);

                        return;
                    }
                }
                else
                {
                    _errorMessage = Operations_Messages.ERR_MSG_INSUR_PER_NOT_FOUND;
                    AuditLogger.RecordFailureInfoLog(_pageName, _currMethodName, _errorMessage);
                    ScriptManager.RegisterStartupScript(this, this.GetType(), "CallMyFunction", "showNotification('bg-black','" + _errorMessage + "','top','center','animated zoomInRight','animated zoomInRight');", true);
                    return;
                }

                AuditLogger.RecordDebugEndLog(_pageName, _currMethodName);
            }
            catch (Exception ex)
            {
                AuditLogger.RecordErrorLog(_pageName, _currMethodName, ex);
                throw ex;
            }
        }

        private void AddImageToPdf(Document document, Bitmap bitmap)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                bitmap.Save(memoryStream, ImageFormat.Png);
                iTextSharp.text.Image pdfImage = iTextSharp.text.Image.GetInstance(memoryStream.ToArray());

                // Scale image to fit the page
                float scale = Math.Min(PageSize.A4.Width / pdfImage.Width, PageSize.A4.Height / pdfImage.Height);
                pdfImage.ScalePercent(scale * 100);

                // Center the image on the page
                pdfImage.SetAbsolutePosition((PageSize.A4.Width - pdfImage.ScaledWidth) / 2, (PageSize.A4.Height - pdfImage.ScaledHeight) / 2);

                document.Add(pdfImage);
            }
        }
        private string GetFormatedAmount(string amt)
        {
            double amount = double.Parse(amt);
            string formattedAmount = "₹ " + amount.ToString("#,##0.00");
            return formattedAmount;
        }
        private string GetFormatedDate(string date)
        {
            if (date.Contains("-"))
            {
                date = date.Replace("-", "/");
            }
            return date;
        }
        private string RemoveAddressDupl(string address)
        {
            string[] words = address.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            HashSet<string> seenWords = new HashSet<string>();
            List<string> resultWords = new List<string>();
            foreach (string word in words)
            {
                if (!seenWords.Contains(word))
                {
                    resultWords.Add(word);
                    seenWords.Add(word);
                }
            }
            string result = string.Join(" ", resultWords);
            return result;
        }
    }
}
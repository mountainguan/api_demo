/// <summary>
/// 简历上传操作
/// </summary>
/// <returns></returns>
[HttpPost("readresume")]
[Authorize]
public JsonResult ImportExcelFile(string userctf, string cid)
{
     string resumeUrl = string.Empty;

     resumeUrl = "http://cv-extract.com/api/extract";

     var client = new HttpClientHepler(resumeUrl);

     dynamic demo =null;

     client.HttpUploadFile(resumeUrl, "resume", "resume.txt", okAction =>
     {
          demo = JsonConvert.DeserializeObject<dynamic>(okAction);
      }).Wait();

      return Json(new { resData = demo });
}

/// 方法代码
public static HttpWebRequest create_request(String url)
{
	HttpWebRequest request = null;
	if (url.StartsWith("https", StringComparison.OrdinalIgnoreCase))
	{
		//对服务端证书进行有效性校验（非第三方权威机构颁发的证书，如自己生成的，不进行验证，这里返回true）
		ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
		request = WebRequest.Create(url) as HttpWebRequest;
		request.ProtocolVersion = HttpVersion.Version10;    //http版本，默认是1.1,这里设置为1.0
	}
	else
	{
		request = WebRequest.Create(url) as HttpWebRequest;
	}
	return request;
}

private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
{
	return true;
}

public async Task HttpUploadFile(string url, string paramName, string file, Action<string> okAction = null,
	Action<Exception> exAction = null)
{
	HttpWebRequest wr = create_request(url);
	string boundary = "---------------------------" + DateTime.Now.Ticks.ToString("x");
	byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");

	wr.ContentType = "multipart/form-data; boundary=" + boundary;
	wr.Method = "POST";
	wr.KeepAlive = false;
	wr.Credentials = System.Net.CredentialCache.DefaultCredentials;

	using (Stream rs = await wr.GetRequestStreamAsync())
	{
		rs.Write(boundarybytes, 0, boundarybytes.Length);
		string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: {2}\r\n\r\n";
		string header = string.Format(headerTemplate, paramName, file, "application/octet-stream");
		byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
		rs.Write(headerbytes, 0, headerbytes.Length);

		using (FileStream fileStream = new FileStream(Path.Combine(@"D:\\config\\A\\", file), FileMode.Open))
		{
			//字段的长度给小一点即可，正常情况不会传入空的附件，因此不需要做较大验证
			byte[] fileBuffer = new byte[2];
			int bytesRead = 0;
			while ((bytesRead = fileStream.Read(fileBuffer, 0, 2)) != 0)
				rs.Write(fileBuffer, 0, 2);
		}

		byte[] trailer = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "--\r\n");
		rs.Write(trailer, 0, trailer.Length);
	}

	try
	{
		using (var wresp = await wr.GetResponseAsync())
		using (Stream responseStream = wresp.GetResponseStream())
		using (StreamReader myStreamReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8")))
			okAction(myStreamReader.ReadToEnd());
	}
	catch (Exception ex)
	{
		exAction?.Invoke(ex);
	}
}
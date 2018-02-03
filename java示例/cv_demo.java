import org.apache.http.HttpEntity;
import org.apache.http.client.methods.CloseableHttpResponse;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.config.Registry;
import org.apache.http.config.RegistryBuilder;
import org.apache.http.conn.HttpClientConnectionManager;
import org.apache.http.conn.socket.ConnectionSocketFactory;
import org.apache.http.conn.socket.PlainConnectionSocketFactory;
import org.apache.http.conn.ssl.SSLConnectionSocketFactory;
import org.apache.http.ssl.SSLContexts;
import org.apache.http.conn.ssl.TrustSelfSignedStrategy;
import org.apache.http.entity.mime.HttpMultipartMode;
import org.apache.http.entity.mime.MultipartEntityBuilder;
import org.apache.http.entity.mime.content.FileBody;
import org.apache.http.impl.client.CloseableHttpClient;
import org.apache.http.impl.client.DefaultRedirectStrategy;
import org.apache.http.impl.conn.PoolingHttpClientConnectionManager;
import org.apache.http.util.EntityUtils;

import java.io.File;
import javax.net.ssl.SSLContext;

public class cv_demo {

    public static String upload_file(String url, String file_path) throws Exception {
        File f = new File(file_path);
        HttpPost httpPost = new HttpPost(url);
        HttpEntity req = MultipartEntityBuilder.create()
                .addPart("resume", new FileBody(f))
                .setMode(HttpMultipartMode.BROWSER_COMPATIBLE)
                .build();
        httpPost.setEntity(req);
        SSLContext sslContext = SSLContexts.custom()
                .loadTrustMaterial(null, new TrustSelfSignedStrategy())
                .build();
        SSLConnectionSocketFactory sslsf = new SSLConnectionSocketFactory(sslContext);
        ConnectionSocketFactory plainSF = new PlainConnectionSocketFactory();
        Registry<ConnectionSocketFactory> r = RegistryBuilder.<ConnectionSocketFactory>create()
                .register("https", sslsf)
                .register("http", plainSF)
                .build();
        HttpClientConnectionManager cm = new PoolingHttpClientConnectionManager(r);
        CloseableHttpClient httpClient = org.apache.http.impl.client.HttpClients.custom()
                .disableCookieManagement()
                .setRedirectStrategy(new DefaultRedirectStrategy())
                .setConnectionManager(cm)
                .build();
        CloseableHttpResponse response = httpClient.execute(httpPost);
        String result = "";
        try {
            HttpEntity responseEntity = response.getEntity();
            if (responseEntity != null) {
                result = EntityUtils.toString(responseEntity, "utf-8");
            }
        } finally {
            response.close();
        }

        return result;
    }

    public static void main(String args[]) {
        String cv_file = "resume.txt";
        String cv_url = "http://cv-extract.com/api/extract";
        try {
            System.out.print(cv_demo.upload_file(cv_url, cv_file));
        } catch (Exception e) {
            e.printStackTrace();
        }

    }
}

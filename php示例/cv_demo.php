<?php
    $url = "http://cv-extract.com/api/extract";
    $file_path = 'resume.txt';
    $file = new \CurlFile($file_path,mime_content_type($file_path));
    $post_data = array(
        "resume" => $file,
    );
    $ch = curl_init();
    curl_setopt($ch , CURLOPT_URL , $url);
    curl_setopt($ch , CURLOPT_RETURNTRANSFER, 1);
    curl_setopt($ch , CURLOPT_POST, 1);
    curl_setopt($ch , CURLOPT_POSTFIELDS, $post_data);
    $output = curl_exec($ch);
    curl_close($ch);
    print_r($output);
?>

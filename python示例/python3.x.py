# python3.x 示例
# _*_ coding:utf-8 _*_
# coding=utf8
import sys
import importlib
import requests

importlib.reload(sys)


# 注:需要安装requests模块


def main():
	cv_file = "resume.txt"
	print(cv_api(cv_file))


def cv_api(cv_file):
	cv_url = "http://resume.com/api/extract"
	return upload_file(cv_url, cv_file)


def upload_file(url, file_path):
	files = {'resume': open(file_path, 'rb')}
	result = requests.post(url, files=files, timeout=15, verify=False)
	return result.text


if __name__ == "__main__":
	main()

package main

import (
	"encoding/json"
	"fmt"
	"io"
	"net/http"
	"os"
	"path/filepath"
	"strings"
)

// 配置结构体
type Config struct {
	Password string `json:"password"`
	Port     string `json:"port"`
}

// 全局变量存储密码和端口
var validPassword string
var serverPort string

func main() {
	// 读取配置文件
	err := loadConfig("config.json")
	if err != nil {
		fmt.Println("加载配置文件失败:", err)
		return
	}

	// 注册路由
	http.HandleFunc("/upload/", uploadFile)     // 上传文件接口
	http.HandleFunc("/download/", downloadFile) // 下载文件接口

	// 启动服务器并监听端口
	fmt.Println("服务器已启动，监听 " + serverPort + " 端口...")
	err = http.ListenAndServe(":"+serverPort, nil)
	if err != nil {
		fmt.Println("启动服务器失败:", err)
	}
}

// 加载配置文件
func loadConfig(filePath string) error {
	// 打开配置文件
	fmt.Println(filePath)
	file, err := os.Open(filePath)
	if err != nil {
		return fmt.Errorf("无法打开配置文件: %v", err)
	}
	defer file.Close()

	// 解析 JSON 配置
	decoder := json.NewDecoder(file)
	var config Config
	err = decoder.Decode(&config)
	if err != nil {
		return fmt.Errorf("无法解析配置文件: %v", err)
	}

	// 设置密码
	validPassword = config.Password
	fmt.Println(config.Password)

	// 设置端口
	serverPort = config.Port
	fmt.Println(serverPort)

	return nil
}

// 删除文件夹内所有内容
func clearDirectory(folderPath string) error {
	files, err := os.ReadDir(folderPath)
	if err != nil {
		return err
	}
	for _, file := range files {
		filePath := filepath.Join(folderPath, file.Name())
		err := os.RemoveAll(filePath) // 删除文件夹内所有文件及子文件夹
		if err != nil {
			return err
		}
	}
	return nil
}

// 验证密码
func checkPassword(r *http.Request) bool {
	// 从查询参数中获取密码
	password := r.URL.Query().Get("password")
	fmt.Println(password)
	// 比对密码
	return password == validPassword
}

// 上传文件处理函数
func uploadFile(w http.ResponseWriter, r *http.Request) {
	// 验证密码
	if !checkPassword(r) {
		http.Error(w, "无效的密码", http.StatusUnauthorized)
		return
	}

	// 限制上传文件大小为 10000 MB
	r.ParseMultipartForm(100000 << 20)

	// 获取文件夹名称参数（从URL路径中提取）
	parts := strings.Split(r.URL.Path, "/")

	if len(parts) < 3 {
		http.Error(w, "文件夹名称不能为空", http.StatusBadRequest)
		return
	}
	folderName := parts[2]

	// 获取上传的文件
	file, header, err := r.FormFile("file")
	if err != nil {
		http.Error(w, "文件上传失败: "+err.Error(), http.StatusInternalServerError)
		return
	}
	defer file.Close()

	fileName := header.Filename

	// 获取当前程序目录并创建目标文件夹路径
	currentDir, err := os.Getwd() // 获取当前工作目录
	if err != nil {
		http.Error(w, "获取当前目录失败: "+err.Error(), http.StatusInternalServerError)
		return
	}

	hotSourceDir := filepath.Join(currentDir, "HotUpdateData")
	dstFolder := filepath.Join(hotSourceDir, folderName)

	// 创建目标文件夹路径
	err = os.MkdirAll(dstFolder, os.ModePerm)
	if err != nil {
		http.Error(w, "创建文件夹失败: "+err.Error(), http.StatusInternalServerError)
		return
	}

	// 目标文件路径
	dstFilePath := filepath.Join(dstFolder, fileName)

	// 仅检查 .bundle 文件是否存在
	if strings.HasSuffix(fileName, ".bundle") {
		if _, err := os.Stat(dstFilePath); err == nil {
			http.Error(w, "文件已存在，不能重复上传", http.StatusConflict)
			return
		}
	}

	// 是否清空文件夹
	if shouldClearFolderBeforeUpload(r) {
		err = clearDirectory(dstFolder)
		if err != nil {
			http.Error(w, "清空文件夹失败: "+err.Error(), http.StatusInternalServerError)
			return
		}
	}

	// 创建目标文件
	dst, err := os.Create(dstFilePath)
	if err != nil {
		http.Error(w, "文件保存失败: "+err.Error(), http.StatusInternalServerError)
		return
	}
	defer dst.Close()

	// 将上传的文件内容拷贝到目标文件
	_, err = io.Copy(dst, file)
	if err != nil {
		http.Error(w, "文件保存失败: "+err.Error(), http.StatusInternalServerError)
		return
	}

	// 返回成功消息
	w.Write([]byte("文件上传成功"))
}

// 验证是否清空文件夹
func shouldClearFolderBeforeUpload(r *http.Request) bool {
	// 从查询参数中获取 ifClearFolder
	ifClear := r.URL.Query().Get("clearFolder")

	// 检查参数是否为 "true"，忽略大小写和空格
	return strings.TrimSpace(strings.ToLower(ifClear)) == "true"
}

// 下载文件处理函数
func downloadFile(w http.ResponseWriter, r *http.Request) {
	// // 验证密码
	// if !checkPassword(r) {
	// 	http.Error(w, "无效的密码", http.StatusUnauthorized)
	// 	return
	// }

	// 获取文件夹和文件名参数（从URL路径中提取）
	parts := strings.Split(r.URL.Path, "/")
	if len(parts) < 4 {
		http.Error(w, "文件夹名称和文件名不能为空", http.StatusBadRequest)
		return
	}
	folderName := parts[2]
	fileName := parts[3]

	// 获取当前程序目录并设置下载文件的路径
	currentDir, err := os.Getwd() // 获取当前工作目录
	if err != nil {
		http.Error(w, "获取当前目录失败: "+err.Error(), http.StatusInternalServerError)
		return
	}

	// 设置文件路径
	filePath := filepath.Join(currentDir, "HotUpdateData", folderName, fileName)

	// 打开文件
	file, err := os.Open(filePath)
	if err != nil {
		http.Error(w, "文件打开失败: "+err.Error(), http.StatusInternalServerError)
		return
	}
	defer file.Close()

	// 获取文件信息
	fileInfo, err := file.Stat()
	if err != nil {
		http.Error(w, "获取文件信息失败: "+err.Error(), http.StatusInternalServerError)
		return
	}

	// 设置响应头，以便浏览器触发下载
	w.Header().Set("Content-Disposition", "attachment; filename="+fileInfo.Name())
	w.Header().Set("Content-Type", "application/octet-stream")
	w.Header().Set("Content-Length", fmt.Sprintf("%d", fileInfo.Size()))

	// 将文件内容发送给客户端
	_, err = io.Copy(w, file)
	if err != nil {
		http.Error(w, "文件下载失败: "+err.Error(), http.StatusInternalServerError)
		return
	}
}

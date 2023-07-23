import socket
import threading

# 连接信息
HOST = '127.0.0.1'
PORT = 11000

# 发送消息


def send_message(sock):
    while True:
        try:
            message = input('Enter message to send: ')
            if message.lower() == "exit":
                break
            send_message_withType(sock,  message, 1)
        except Exception as e:
            print('Error:', e)
            break

    # 关闭套接字
    sock.close()


def send_message_withType(sock, message, message_type):
    # 构造消息头
    header = '{:04d}{:d}'.format(len(message), 1)
    header = header.encode('utf-8')

    # 发送消息
    message = message.encode('utf-8')
    message = header + message
    sock.sendall(message)
# 接收消息


# 定义从服务器接收消息的函数
def recv_message(sock):
    while True:
        try:
            # 接收服务器的消息
            data = sock.recv(1024)

            # 如果服务器关闭了连接，退出循环
            if not data:
                break

            # 打印服务器发送的消息
            print('Received message:', data.decode())
        except Exception as e:
            print('Error:', e)
            break

    # 关闭客户端套接字
    sock.close()


# 连接服务器
sock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
sock.connect((HOST, PORT))
print('Connected to', (HOST, PORT))

# 启动发送和接收线程
send_thread = threading.Thread(target=send_message, args=(sock,))
recv_thread = threading.Thread(target=recv_message, args=(sock,))
send_thread.start()
recv_thread.start()

use std::sync::LazyLock;
use tokio::{io::AsyncWriteExt, net::TcpStream};

static CLIENT: LazyLock<reqwest::Client> = LazyLock::new(|| reqwest::Client::new());
#[allow(non_upper_case_globals)]
static OUTPUT_COUNT_ÙwÚ: usize = 100;

async fn make_http_request(url: &str) -> Vec<String> {
    // Make the HTTP GET request using ureq
    CLIENT.get(url).send().await
        .unwrap()
        .json().await.unwrap()
}

#[tokio::main]
async fn main() {
    let (sender, receiver) = async_channel::unbounded();

    tokio::spawn({
        let sender = sender.clone();
        async move {
            loop {
                let result = make_http_request("http://151.217.2.77:5000/instructions/").await;
                let _ = sender.send(result).await;
            }
        }
    });

    for _ in 0..OUTPUT_COUNT_ÙwÚ {
        tokio::spawn({
            let receiver = receiver.clone();
            async move {
                loop {
                    match TcpStream::connect("151.217.15.90:1337").await {
                        Ok(mut stream) => {
                            let commands = receiver.recv().await.unwrap();
                            for command in commands {
                                let _ = stream.write_all(command.as_bytes()).await;
                                let _ = stream.write_all(b"\n").await;
                            }

                            let _ = stream.flush().await;
                        }
                        Err(err) => {
                            eprintln!("connection failed: {err}");
                        }
                    }
                }
            }
        });
    }

    std::future::pending::<()>().await;
}

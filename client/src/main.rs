use bufstream::BufStream;
use http::Uri;
use serde::Deserialize;
use std::sync::mpsc;
use std::thread;
use std::{io::Write, net::TcpStream};
fn make_http_request(url: &str) -> Vec<String> {
    // Parse the URL into a Uri
    let uri: Uri = url.parse().expect("Invalid URL");

    // Make the HTTP GET request using ureq
    return ureq::get(&uri.to_string())
        .call()
        .unwrap()
        .into_json()
        .unwrap();
}

fn ballern(commands: Vec<String>, buffer: BufStream<TcpStream>) {
    for cmd in commands {
        buffer.get_ref().write_all(cmd.as_bytes()).unwrap();
        buffer.get_ref().write_all(b"\n").unwrap();
        buffer.get_ref().flush().unwrap();
    }
}

fn main() {
    let (tx, rx) = mpsc::channel();

    thread::spawn(move || {
        while (true) {
            tx.send(make_http_request("http://151.217.2.77:5000/Instructions"));
        }
    });
    thread::spawn(move || {
        while (true) {
            match TcpStream::connect("151.217.15.90:1337") {
                Ok(mut stream) => {
                    let mut buf = BufStream::new(stream);
                    ballern(rx.recv().unwrap(), buf);
                }
                Err(e) => {
                    println!("Failed to connect: {}", e);
                }
            }
        }
    });
    while (true) {}
}

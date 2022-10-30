// -*- coding:utf-8-unix -*-

use proconio::source::line::LineSource;
use std::{
    io::{stdin, BufReader},
    time::Instant,
};

fn main() {
    let clock = Instant::now();

    let _stdin = stdin();
    let mut _source = LineSource::new(BufReader::new(_stdin.lock()));
    macro_rules! input(
        ($($tt:tt)*) => (proconio::input!(from &mut _source, $($tt)*))
    );

    input! {
        n: usize,
        v: [isize; n]
    };

    println!("{}", v[0]);
}

#[allow(unused_macros)]
macro_rules! mat {
	($($e:expr),*) => { Vec::from(vec![$($e),*]) };
	($($e:expr,)*) => { Vec::from(vec![$($e),*]) };
	($e:expr; $d:expr) => { Vec::from(vec![$e; $d]) };
	($e:expr; $d:expr $(; $ds:expr)+) => { Vec::from(vec![mat![$e $(; $ds)*]; $d]) };
}

trait ChMinMax {
    fn chmin(&mut self, v: Self) -> bool;
    fn chmax(&mut self, v: Self) -> bool;
}
impl<T> ChMinMax for T
where
    T: PartialOrd,
{
    fn chmin(&mut self, v: T) -> bool {
        *self > v && {
            *self = v;
            true
        }
    }
    fn chmax(&mut self, v: T) -> bool {
        *self < v && {
            *self = v;
            true
        }
    }
}

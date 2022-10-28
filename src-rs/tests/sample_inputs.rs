// use cli_test_dir::*;

// const BIN: &'static str = "./main";

// #[test]
// fn sample1() {
//     let testdir = TestDir::new(BIN, "");
//     let output = testdir
//         .cmd()
//         .output_with_stdin(r#"2
// 3 1 2
// 6 1 1
// "#)
//         .tee_output()
//         .expect_success();
//     assert_eq!(output.stdout_str(), "Yes\n");
//     assert!(output.stderr_str().is_empty());
// }

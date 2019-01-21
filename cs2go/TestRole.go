package main

type TestRole struct {
	Attack  int
	Defense int
	Flg     bool
	Strs    []int
	d       map[string]int
}

var Name = "MyName"

func (tn *TestRole) GetHurt(testRole TestRole, callNum int) int {
	var num1 = 101
	for i := 0; i < 10; i++ {
		num1++
	}
	return 0
}
func (tn *TestRole) TestFunc(num int, name string) int {
	return 100
}
func ShowTest(num int) {
}
func (tn *TestRole) Show() {
}
func QQQShow() {
}

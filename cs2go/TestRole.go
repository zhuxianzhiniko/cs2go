package main

type TestRole struct {
	Attack  int
	Defense int
}

var Name = "sss"

func (tn *TestRole) GetHurt(testRole TestRole, callNum int) int {
	tn.Show()
	tn.QQQShow()
	var num = 100
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

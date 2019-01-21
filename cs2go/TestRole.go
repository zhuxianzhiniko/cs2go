package main

type TestRole struct {
	Attack  int
	d       map[string]int
	Defense int
	Flg     bool
	Strs    []int
}

var Name = "MyName"

func (tn *TestRole) GetHurt(testRole TestRole, callNum int) int {
	tn.Show()
	StShow()
	var num = 100
	var role = new(TestRole)
	var dd = num*100 + 100
	var num3 = num + 1
	var len = cap(role.Strs)
	var num1 = 101
	for i := 10; i > 100; i++ {
		num1++
	}
	return cap(testRole.Strs)
}
func (tn *TestRole) TestFunc(num int, name string) int {
	return 100
}
func ShowTest(num int) int {
	return 100
}
func (tn *TestRole) Show() {
}
func StShow() {
}

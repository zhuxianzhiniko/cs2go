package main

type TestRole struct {
	Attack  int
	Defense int
	Name    string
}

func (tn *TestRole) GetHurt(testRole TestRole, callNum int) int {
	tn.Show()
	var num = 100
	var num1 = 101
	num += 1
	var dd = num*100 + 100
	var num3 = num + num1
	var role = new(TestRole)
	return callNum - testRole.Defense
}
func (tn *TestRole) TestFunc(num int, name string) int {
	return 100
}
func (tn *TestRole) ShowTest(num int) {
}
func (tn *TestRole) Show() {
}

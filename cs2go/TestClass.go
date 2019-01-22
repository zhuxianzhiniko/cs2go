package main

type TestClass struct {
	Attack  int
	d       map[string]int
	Defense int
	Flg     bool
	Strs    []int
}

var Name = "MyName"

func Init() *TestClass {
	var testClass = new(TestClass)
	testClass.Attack = 10
	testClass.Defense = 20
	testClass.Flg = false
	return testClass
}
func (tn *TestClass) GetHurt(testClass TestClass, callNum int) int {
	tn.Show()
	StShow()
	var num = 100
	var test = new(TestClass)
	var dd = num*100 + 100
	var num3 = num + 1
	var len = cap(test.Strs)
	var num1 = 101
	for i := 10; i > 100; i++ {
		num1++
	}
	return cap(testClass.Strs)
}
func (tn *TestClass) TestFunc(num int, name string) int {
	return 100
}
func ShowTest(num int) int {
	return 100
}
func (tn *TestClass) Show() {
}
func StShow() {
}
func (tn *TestClass) GetMaxAttack() int {
	return 100
}

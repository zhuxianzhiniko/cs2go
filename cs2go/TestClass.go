package main

type MyTestEnum int

const (
	One1 MyTestEnum = iota
	Tow1
)

func (th *TestClass) Show() {

}
func (th *TestClass) GetHurt(list []int, testClass TestClass, callNum int) int {
	th.dic[100] = "dd"
	return 0

}

type TestClass struct {
	dic  map[int]string
	list []int
}

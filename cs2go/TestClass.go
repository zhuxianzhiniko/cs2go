package main

type MyTestEnum int

const (
	One1 MyTestEnum = iota
	Tow1
)

func (th *TestClass) GetHurt(list []int, testClass TestClass, callNum int) int {
	var d = []int{1, 2}
	var d1 = []int{1, 2, 3, 4}
	var listLen = cap(d1)
	var arrLen = cap(d)
	return cap(d)

}

type TestClass struct {
}

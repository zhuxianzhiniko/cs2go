package main

type MyTestEnum int

const (
	One1 MyTestEnum = iota
	Tow1
)

func (th *TestClass) Show() {

}
func (th *TestClass) GetHurt(list []int, testClass TestClass, callNum int) int {
	var d1 = []int{1, 2, 3, 4}
	d1 = append(d1[:0], d1[0+1:]...)
	d1 = append(d1, 100)
	return cap(d1)

}

type TestClass struct {
}

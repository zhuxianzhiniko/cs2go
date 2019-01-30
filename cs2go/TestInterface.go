package main

type TestInterface interface {
	InterfaceShow()
	InterfaceShow1(num int)
	GetMaxAttack(name string, key int) int
}

func (th *TestClass1) Show() {

}
func (th *TestClass2) Show() {

}

type TestClass1 struct {
	num int
}
type TestClass2 struct {
	num intg
}

package main

type MyTestEnum int

const (
	One MyTestEnum = iota
	Tow
)
const Max = 100
const Name = "MyName"
const OnlyFlg = false

func Init() *TestClass {
	var testClass = new(TestClass)
	testClass.Attack = 10
	testClass.Defense = 20
	testClass.Flg = false
	return testClass

}
func (th *TestClass) GetHurt(list []string, testClass TestClass, callNum int) int {
	var testVar = 0
	var TestInt int = 0
	var TestUint uint32 = 0
	var testshort int16 = 0
	var testUshort uint16 = 0
	var testUulong uint64 = 0
	var testUlong int64 = 0
	var testFloat float32 = 0
	var testdouble float64 = 0
	var str string = "sss"
	var flg bool = false
	var myTestEnum MyTestEnum = One
	var TestArray []int = []int{1, 2}
	var TestList []int = []int{1, 2, 3, 4}
	var TestList1 []string = []string{"false", "false"}
	var listLen = cap(TestList)
	var arrLen = cap(TestArray)
	TestList = append(TestList[:0], TestList[0+1:]...)
	TestList = append(TestList, 100)
	TestList = TestList[:0:0]
	th.dic[100] = "dd"
	var b = list[0]
	th.dic[100] = list[0]
	StShow()
	ShowTest(100)
	var attack = th.GetMaxAttack(100)
	th.GetMaxAttack(100)
	testClass.Show
	var test = new(TestClass)
	var test1 = Init()
	test1.Show
	var num1 = 100 + 2 - 10*100/2
	for i := 10; i < cap(testClass.lists); i++ {
		i++

	}

	if th.myTestEnum == One || testClass.dic[100] != "test" && th.myTestEnum == Tow {
		th.dic[100] = "dd"

	} else if th.myTestEnum == Tow {
		th.dic[100] = "dd1"

	} else {
		th.dic[100] = "dd2"

	}

	switch testClass.GetMaxAttack {
	case 1:
		for i := 10; i < 100; i++ {
			i++
			for k := 10; k < 100; k++ {
				k++
				switch 1 {
				case 1:
					break

				case 2:
					if th.myTestEnum == One || testClass.dic[100] != "test" && th.myTestEnum == Tow {
						th.dic[100] = "dd"

					} else if th.myTestEnum == Tow {
						th.dic[100] = "dd1"

					} else {
						th.dic[100] = "dd2"

					}

					break

				}

			}

		}

		break

	case 2:
		var k1 = 1
		k1 += 1
		break

	case 3:
		var k2 = 1
		k2 -= 1
		break

	default:
		var k3 = 1
		k3 /= 1
		break

	}

	return 0

}
func (th *TestClass) Show() {

}
func (th *TestClass) GetMaxAttack(num int) int {
	return 100

}
func ShowTest(num int) int {
	return 100

}
func StShow() {

}

type TestClass struct {
	dic        map[int]string
	lists      []int
	Attack     int
	Defense    int
	Flg        bool
	myTestEnum MyTestEnum
}

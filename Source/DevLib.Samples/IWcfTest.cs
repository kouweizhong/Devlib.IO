﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Runtime.Serialization;
using System.ServiceModel.Web;

namespace DevLib.Samples
{
    [ServiceContract]
    public interface IWcfTest
    {
        [OperationContract]
        //[WebGet]
        string Echo();

        [OperationContract]
        string MyOperation1(string arg1, int arg2);

        [OperationContract]
        void MyOperation2(string value);

        [OperationContract]
        object Foo(string value);

        [OperationContract]
        void AddAnimal(Animal value);

        [OperationContract(IsOneWay = true)]
        void TestOneWay(string a);

        [OperationContract(IsOneWay = true)]
        void TestOneWayException(string a);

        [OperationContract(IsOneWay = false)]
        void TestException(string a);
    }

    [DataContract]
    public class Animal
    {
        [DataMember]
        public string Name { get; set; }
    }

    [DataContract]
    public class Cat : Animal
    {
        [DataMember]
        public string A { get; set; }
    }

    [DataContract]
    public class Dog : Animal
    {
        [DataMember]
        public string B { get; set; }
    }

    [DataContract]
    public class Bird : Animal
    {
        [DataMember]
        public string C { get; set; }
    }
}

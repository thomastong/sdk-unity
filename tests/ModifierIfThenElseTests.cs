using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using NMock2;
using Roar.Components;
using Roar.DomainObjects;
using Roar.DomainObjects.Modifiers;
using Roar.implementation.DataConversion;

/**
 * Test cases for the Modifier If-Then-Else component
 **/

[TestFixture()]
public class ModifierIfThenElseTest
{

  private Mockery mockery = null;

  [SetUp]
  public void TestInitialise()
  {
    this.mockery = new Mockery();
  }
  
  [Test()]
  public void TestGetsIfThenElse()
  {
    XCRMParser parser = new XCRMParser();
    IXMLNode ixmlnode = mockery.NewMock<IXMLNode>();
    List<IXMLNode> if_then_else_nodes = new List<IXMLNode>();
    IXMLNode if_node = mockery.NewMock<IXMLNode>();
    IXMLNode then_node = mockery.NewMock<IXMLNode>();
    IXMLNode else_node = mockery.NewMock<IXMLNode>();
    Expect.AtLeastOnce.On(if_node).GetProperty("Name").Will(Return.Value("if"));
    Expect.AtLeastOnce.On(then_node).GetProperty("Name").Will(Return.Value("then"));
    Expect.AtLeastOnce.On(else_node).GetProperty("Name").Will(Return.Value("else"));
    if_then_else_nodes.Add(if_node);
    if_then_else_nodes.Add(then_node);
    if_then_else_nodes.Add(else_node);
    Expect.AtLeastOnce.On(ixmlnode).GetProperty("Name").Will(Return.Value("if_then_else"));
    Expect.AtLeastOnce.On(ixmlnode).GetProperty("Children").Will(Return.Value(if_then_else_nodes));
    
    parser.crm = mockery.NewMock<IXCRMParser>();
    List<Roar.DomainObjects.Requirement> mock_if_requirement_list = new List<Roar.DomainObjects.Requirement>();
    List<Roar.DomainObjects.Modifier> mock_then_modifier_list = new List<Roar.DomainObjects.Modifier>();
    List<Roar.DomainObjects.Modifier> mock_else_modifier_list = new List<Roar.DomainObjects.Modifier>();
    Expect.AtLeastOnce.On(parser.crm).Method("ParseRequirementList").With(if_node).Will(Return.Value(mock_if_requirement_list));
    Expect.AtLeastOnce.On(parser.crm).Method("ParseModifierList").With(then_node).Will(Return.Value(mock_then_modifier_list));
    Expect.AtLeastOnce.On(parser.crm).Method("ParseModifierList").With(else_node).Will(Return.Value(mock_else_modifier_list));
    
    Modifier m = parser.ParseAModifier(ixmlnode);
    mockery.VerifyAllExpectationsHaveBeenMet();
    Assert.IsNotNull(m as IfThenElse);
    Assert.AreSame((m as IfThenElse).if_, mock_if_requirement_list);
    Assert.AreSame((m as IfThenElse).then_, mock_then_modifier_list);
    Assert.AreSame((m as IfThenElse).else_, mock_else_modifier_list);
  }
  
}

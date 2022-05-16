using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace reflecBug;

public partial class MainPage : ContentPage {

	public MainPage() {
		InitializeComponent();

		var item = new MyItem();
		item.Id = 100;

		try {
			Console.WriteLine(GetValue(item));
		} catch(Exception e) {
			//Failed on iOS (Release [AOT mode])
			label.Text = e.ToString();
			Console.WriteLine(e);
		}
	}


	public object GetValue(object component) {
		Func<object, object> propertyAccessorDelegate = CompileLambda();
        return propertyAccessorDelegate(component);
    }

	Func<object, object> CompileLambda() {
        ParameterExpression parameter = Expression.Parameter(typeof(object), "p");
        MemberExpression property = Expression.Property(Expression.Convert(parameter, typeof(MyItem)), "Id");
        Expression<Func<object, object>> lambda = Expression.Lambda<Func<object, object>>(Expression.Convert(property, typeof(object)), parameter);
        return lambda.Compile();
    }
}


public class BaseItem {
    int id;

    public virtual int Id {
        get => id;
        set => this.id = value;
    }
}

public class MyItem: BaseItem {
    int id;

	//Some attributes....
    public override int Id { get => base.Id; set => base.Id = value; }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TextEditor.Strategies;
using TextEditor.UserControls;
using System.Windows.Controls;
using TextEditor.Models;

public class DefaultUserControlSwitchStrategy : IUserControlSwitchStrategy
{
    private readonly IUserControlFactory factory;

    public DefaultUserControlSwitchStrategy(IUserControlFactory factory)
    {
        this.factory = factory;
    }

    public UserControl GetUserControl(UserControlTypes type, Document document)
    {
        return factory.CreateUserControl(type, document, null);
    }
}

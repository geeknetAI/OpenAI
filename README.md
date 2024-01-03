# OpenAI

**OpenAI’s Assistants API’s - A hands-on demo in .NET CORE to build interactive Chat application **

The Assistants API by OpenAI allows you to create AI-powered assistants in your applications. These assistants can understand and respond to user queries using AI models and various tools. The assistants also retain the context and historical information. 

**The other key features include:**
    
    Customization: You can define specific instructions and choose an AI model for your assistant.
    Tools: It supports tools like Code Interpreter, Retrieval, and Function calling to enhance the assistant's capabilities.
    Usage Flow: To use it, you create an assistant, start a conversation thread, add user messages to the thread, and run the assistant to generate responses.
		
For more details regarding Assistants API, please refer https://platform.openai.com/docs/assistants/overview.

The complete source code can be downloaded from the following GitHub repository: https://github.com/geeknetAI/OpenAI

**Pre-requisite**:

1.	Set up Open AI API Key
2.	Set up a new assistant in the Open AI and note the assistant id.

In this demo, we will create an interactive chat assistant in .NET Core. Let’s discuss the steps involved to invoke different Assistant API’s. 

**Step 1: Create an Assistant**: 

This is the initial setup step where you define the characteristics of your AI assistant. 
Since we already set up new assistant in the prerequisite step above, we will need an API call to fetch this assistant and create an assistant object to be used in further step.

**Step 2: Create a Thread:** 
When a user initiates a conversation with the assistant, a "thread" is created. This thread can be seen as a continuous conversation stream to which both the user and the assistant contribute.

**Step 3: Add Messages to the Thread:** 
As the user interacts with the assistant, their queries or statements are added to the thread as messages.

**Step 4: Run the Assistant on the Thread:** 

This step involves activating the assistant to process the messages in the thread and generate responses. When the assistant runs:

    •	It evaluates the user's messages.
    •	It automatically invokes the relevant tools based on the content of the messages and the assistant's configuration.



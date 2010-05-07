
#include "pch.h"
#include "Command.h"

namespace tio
{
	using namespace std;
	using boost::shared_ptr;

	template<typename T>
	void dump_container(const T& c, ostream& stream)
	{
		stream << "(";

		if(c.size() != 0)
		{
			typename T::const_iterator i = c.begin();

			for(;;)
			{
				stream << *i;
				++i;

				if(i == c.end())
					break;

				stream << ",";
			}
		}

		stream << ")" << endl;
		return;
	}


	Command::Command():
	separators_(" ")
	{}

	void Command::Parse(const char* source)
	{
		boost::split(params_, source, boost::is_any_of(separators_));

		if(params_.size() == 0)
		{
			params_.clear();
			command_.clear();
			throw std::invalid_argument("not a command");
		}

		command_ = params_[0];
		params_.erase(params_.begin());
	}

	const string& Command::GetCommand() const
	{
		return command_;
	}

	const vector<string>& Command::GetParameters() const
	{
		return params_;
	}

	void Command::Dump(ostream& stream) const
	{
		stream << command_ << endl;

		dump_container(params_, stream);
	}

	shared_ptr<tio::Buffer>& Command::GetDataBuffer()
	{
		if(!data_)
			data_ = shared_ptr<tio::Buffer>(new tio::Buffer());

		return data_;

	}
	void Command::SetDataBuffer(const shared_ptr<tio::Buffer>& data)
	{
		data_ = data;
	}
}

#pragma once
#ifndef SHAPE
#define SHAPE

#include <iostream>

class Shape
{
public:
	Shape() = default;
	Shape(const Shape& shape) {};
	Shape& operator=(const Shape& rhs) {}
	~Shape() = default;
	virtual double getArea() abstract;
	virtual Shape& clone() abstract;
	virtual Shape& getFrameRect() abstract;
	virtual void move(double delta_x, double delta_y) abstract;
	virtual void moveTo(double x, double y) abstract;
	virtual void scale(double x, double y, double sigma) abstract;
	virtual std::string getName() abstract;
	virtual double getMaxWidth() abstract;
	virtual double getMaxHeigth() abstract;
	virtual double getPoints(int index) abstract;
	virtual double getX() abstract;
	virtual double getY() abstract;
};

#endif // !SHAPE
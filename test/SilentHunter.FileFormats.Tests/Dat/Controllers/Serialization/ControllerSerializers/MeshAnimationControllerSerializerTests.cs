using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using FluentAssertions;
using Moq;
using SilentHunter.Controllers;
using SilentHunter.Testing.FluentAssertions;
using Xunit;

namespace SilentHunter.FileFormats.Dat.Controllers.Serialization
{
	public class MeshAnimationControllerSerializerTests
	{
		private readonly MeshAnimationControllerSerializer _sut;
		private readonly MeshAnimationController _controller;

		public MeshAnimationControllerSerializerTests()
		{
			_sut = new MeshAnimationControllerSerializer();
			_controller = new Mock<MeshAnimationController>().Object;
		}

		[Fact]
		public void Given_invalid_controller_type_when_serializing_should_throw()
		{
			// Act
			Action act = () => _sut.Serialize(Stream.Null, new Mock<Controller>().Object);

			// Assert
			act.Should()
				.Throw<InvalidOperationException>()
				.WithMessage($"Expected controller of type {nameof(MeshAnimationController)}.");
		}

		[Fact]
		public void Given_invalid_controller_type_when_deserializing_should_throw()
		{
			// Act
			Action act = () => _sut.Deserialize(Stream.Null, new Mock<Controller>().Object);

			// Assert
			act.Should()
				.Throw<InvalidOperationException>()
				.WithMessage($"Expected controller of type {nameof(MeshAnimationController)}.");
		}

		[Fact]
		public void Given_too_many_frames_when_serializing_should_throw()
		{
			_controller.Frames = Enumerable.Repeat(new AnimationKeyFrame(), ushort.MaxValue + 1).ToList();

			// Act
			Action act = () => _sut.Serialize(Stream.Null, _controller);

			// Assert
			act.Should()
				.Throw<InvalidOperationException>()
				.WithMessage($"Mesh animations can only support up to {ushort.MaxValue} frames.");
		}

		[Fact]
		public void Given_mesh_animation_controller_when_serializing_and_then_deserializing_should_give_equivalent()
		{
			byte[] expectedSerializedData = {
				0x02, 0x00, 0x48, 0xe1, 0x7a, 0x3f, 0x00, 0x00, 0x00, 0x00, 0xc0, 0x3f, 0x01, 0x00, 0x02, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x00, 0xc0, 0x00, 0xc0, 0x38, 0xc0, 0x00, 0x40, 0x40, 0xab, 0xaa, 0x55, 0xd5, 0x00, 0x00, 0xaa, 0x2a, 0x55, 0x55, 0xff, 0x7f, 0xc0, 0x00, 0xc0, 0x38, 0x40, 0xff, 0x3f, 0xc0, 0x55, 0xd5, 0xab, 0xaa, 0x00, 0x80, 0x55, 0x55, 0xaa, 0x2a, 0x00, 0x00, 0x10, 0xff, 0xc0
			};
			_controller.Frames = new List<AnimationKeyFrame>
			{
				new AnimationKeyFrame
				{
					FrameNumber = 0,
					Time = 0.98f
				},
				new AnimationKeyFrame
				{
					FrameNumber = 1,
					Time = 1.5f
				}
			};
			_controller.CompressedFrames = new List<CompressedVectors>
			{
				VectorCompression.Compress(
					new List<Vector3>
					{
						new Vector3(1, 2, 3),
						new Vector3(4, 5, 6)
					}
				),
				VectorCompression.Compress(
					new List<Vector3>
					{
						new Vector3(-4, -5, -6),
						new Vector3(-1, -2, -3)
					}
				)
			};
			_controller.Unknown0 = new byte[] { 0x10, 0xFF, 0xC0};

			MeshAnimationController deserializedController = new Mock<MeshAnimationController>().Object;

			// Act
			using (var ms = new MemoryStream())
			{
				_sut.Serialize(ms, _controller);
				ms.Position = 0;

				_sut.Deserialize(ms, deserializedController);

				// Assert
				ms.Should().BeEof();
				ms.ToArray().Should().BeEquivalentTo(expectedSerializedData);
				_controller.Should().BeEquivalentTo(deserializedController);
			}
		}
	}
}
